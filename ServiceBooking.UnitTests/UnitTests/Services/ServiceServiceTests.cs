using Application.Common.page;
using Application.DTOs.Service;
using Application.Interfaces.Services.IfileService;
using Application.Interfaces.Services.Implement;
using AutoMapper;
using Domain.Interfaces.UnitofWork;
using Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace ServiceBooking.UnitTests.UnitTests.Services
{
    public class ServiceServiceTests
    {
        private readonly Mock<IUnitofWork> _unitofWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly ServiceService _serviceService;


        public ServiceServiceTests()
        {
            _mapperMock = new Mock<IMapper>();
            _unitofWorkMock = new Mock<IUnitofWork>();
            _fileServiceMock = new Mock<IFileService>();

            // ربط الـ Properties الخاصة بالـ UnitOfWork لتجنب الـ NullReference
            _unitofWorkMock.Setup(u => u.Services).Returns(new Mock<Domain.Interfaces.Repositories.IServiceRepository>().Object);
            _unitofWorkMock.Setup(u => u.Providers).Returns(new Mock<Domain.Interfaces.Repositories.IProviderRepository>().Object);

            _serviceService = new ServiceService(_unitofWorkMock.Object, _mapperMock.Object, _fileServiceMock.Object);
        }
        [Fact]
        public async Task GetServiceById_ShouldReturnService_WhenExists()
        {
            // Arrange
            var serviceId = 1;
            var fakeService = new Service("House Cleaning", 150.5m, "Desc", 60);
            var readDto = new ServiceReadDto { Title = "House Cleaning", Price = 150.5m };

            _unitofWorkMock.Setup(u => u.Services.GetByIdAsync(serviceId)).ReturnsAsync(fakeService);
            _mapperMock.Setup(m => m.Map<ServiceReadDto>(fakeService)).Returns(readDto);

            // Act
            var result = await _serviceService.GetByIdAsync(serviceId);

            // Assert
            result.Should().NotBeNull();
            result!.Title.Should().Be("House Cleaning");
            _unitofWorkMock.Verify(u => u.Services.GetByIdAsync(serviceId), Times.Once);
        }

        [Fact]
        public async Task CreateServiceAsync_ShouldAddServiceAndUploadImage_WhenDataIsValid()
        {
            var userId = "provider-99";
            var dto = new ServiceCreateDto
            {
                Title = "Full Home Maintenance",
                Price = 500,
                ImageFiles = new List<IFormFile> { new Mock<IFormFile>().Object },
                IsPrimaryStatus = new List<bool> { true }
            };

            var fakeProvider = new Provider(userId, "Clean Co.");
            var serviceEntity = new Service(dto.Title, dto.Price, "Desc", 120);

            _unitofWorkMock.Setup(u => u.Providers.FindAsync(It.IsAny<Expression<Func<Provider, bool>>>()))
                                       .ReturnsAsync(new List<Provider> { fakeProvider });

            _mapperMock.Setup(m => m.Map<Service>(dto)).Returns(serviceEntity);
            _fileServiceMock.Setup(f => f.UploadFileAsync(It.IsAny<IFormFile>(), "services")).ReturnsAsync("img1.jpg");

            // إعداد الـ UnitOfWork
            _unitofWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _unitofWorkMock.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);
            _unitofWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // Act
            await _serviceService.CreateAsync(dto, userId);
            // Assert
            fakeProvider.ProviderServices.Should().Contain(ps => ps.Price == dto.Price);
            _unitofWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenImagesAndStatusCountsMismatch()
        {
            // Arrange
            var userId = "provider-99";
            var dto = new ServiceCreateDto
            {
                Title = "Test",
                ImageFiles = new List<IFormFile> { new Mock<IFormFile>().Object },
                IsPrimaryStatus = new List<bool>() // Mismatch
            };

            var fakeProvider = new Provider(userId, "Clean Co.");
            _unitofWorkMock.Setup(u => u.Providers.FindAsync(It.IsAny<Expression<Func<Provider, bool>>>()))
                           .ReturnsAsync(new List<Provider> { fakeProvider });

            // Act
            var action = () => _serviceService.CreateAsync(dto, userId);

            // Assert
            await action.Should().ThrowAsync<ArgumentException>()
                        .WithMessage("*Mismatch*");
        }


        [Fact]
        //this 
        public async Task UpdateAsync_ShouldUpdateDataAndManageImages_WhenDataIsValid()
        {
            // 1. Arrange
            var serviceId = 10;
            var userId = "user-123";
            var oldImageId = 55; // رقم مميز للصورة القديمة

            var existingService = new Service("Old Title", 100, "Old Desc", 30);
            //  give the service id manually 
            typeof(Service).GetProperty("Id")?.SetValue(existingService, serviceId);

            existingService.AddImage("old-image.jpg", true);
            var oldImage = existingService.Images.First();

            // give the oldimage id manaully 
            typeof(Image).GetProperty("Id")?.SetValue(oldImage, oldImageId);
            // provider 
            var fakeProvider = new Provider(userId, "Test Provider");
            fakeProvider.AddService(serviceId, 100);
            var dto = new ServiceUpdateDto
            {
                Title = "New Title",
                Price = 150,
                ImageIdsToDelete = new List<int> { oldImageId }, // هنمسح الصورة القديمة
                NewImageFiles = new List<IFormFile> { new Mock<IFormFile>().Object }, // هنضيف صورة جديدة
                NewIsPrimaryStatus = new List<bool> { true }
            };
            // Mocks Setup
            _unitofWorkMock.Setup(u => u.Services.GetByIdWithImagesAsync(serviceId)).ReturnsAsync(existingService);
            _unitofWorkMock.Setup(u => u.Providers.GetByUserIdWithDetailsAsync(userId)).ReturnsAsync(fakeProvider);
            _fileServiceMock.Setup(f => f.UploadFileAsync(It.IsAny<IFormFile>(), "services")).ReturnsAsync("new-image.jpg");

            _unitofWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _unitofWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            // 2. Act
            var result = await _serviceService.UpdateAsync(serviceId, dto, userId);

            // 3. Assert
            // التأكد من تعديل البيانات الأساسية
            existingService.Title.Should().Be("New Title");

            // التأكد من حذف الصورة القديمة من الـ Collection
            existingService.Images.Should().NotContain(i => i.Id == oldImageId);

            // التأكد من إضافة الصورة الجديدة
            existingService.Images.Should().Contain(i => i.ImagePath == "new-image.jpg");

            // التأكد من تحديث السعر عند البروفايدر
            fakeProvider.ProviderServices.First(ps => ps.ServiceId == serviceId).Price.Should().Be(150);

            // التأكد من نداء ميثود حذف الملف من الهارد ديسك
            _fileServiceMock.Verify(f => f.DeleteFile(It.Is<string>(path => path.Contains("old-image.jpg"))), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowUnauthorizedException_WhenProviderDoesNotExist()
        {
            // 1. Arrange
            var serviceId = 1;
            var userId = "wrong-user-id";
            var dto = new ServiceUpdateDto { Title = "Any Title", Price = 100 };

            // بنجهز خدمة موجودة فعلاً عشان الكود يتخطى أول سطر بنجاح
            var existingService = new Service("Service Title", 100, "Desc", 30);
            _unitofWorkMock.Setup(u => u.Services.GetByIdWithImagesAsync(serviceId))
                           .ReturnsAsync(existingService);

            // make the mock return provider => null
            _unitofWorkMock.Setup(u => u.Providers.GetByUserIdWithDetailsAsync(userId))
                   .ReturnsAsync((Provider)null!);
            //.Act
            var action = () => _serviceService.UpdateAsync(serviceId, dto, userId);

            //  Assert
            await action.Should().ThrowAsync<UnauthorizedAccessException>()
                        .WithMessage("Provider not authorized.");

            _unitofWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Never);
            _unitofWorkMock.Verify(u => u.CompleteAsync(), Times.Never);
        }
        [Fact]
        // test the delete if this service is have by only one provider 
        public async Task DeleteAsync_ShouldFullDeleteServiceAndFiles_WhenNoOtherProvidersExist()
        {
            // 1. Arrange
            var serviceId = 1;
            var userId = "unique-provider";
            var fakeProvider = new Provider(userId, "Unique Provider");

            var serviceToDelete = new Service("Service Title", 100);
            serviceToDelete.AddImage("img1.jpg", true);

            _unitofWorkMock.Setup(u => u.Providers.GetByUserIdWithDetailsAsync(userId))
                   .ReturnsAsync(fakeProvider);
            _unitofWorkMock.Setup(u => u.Services.GetByIdWithImagesAsync(serviceId))
                           .ReturnsAsync(serviceToDelete);
            // empty list this mean there is not provider 
            _unitofWorkMock.Setup(u => u.ProviderServices.FindAsync(It.IsAny<Expression<Func<ProviderService, bool>>>()))
                   .ReturnsAsync(new List<ProviderService>());


            _unitofWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _unitofWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // 2. Act
            var result = await _serviceService.DeleteAsync(serviceId, userId);

            // 3. Assert
            result.Should().BeTrue();

            // sure that the service delete 
            _unitofWorkMock.Verify(u => u.Services.DeleteAsync(serviceToDelete), Times.Once);

            // delete it from the hard disk
            _fileServiceMock.Verify(f => f.DeleteFile(It.Is<string>(path => path.Contains("img1.jpg"))), Times.Once);

            _unitofWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);


        }

        [Fact]
        //Delete the link only (when other provider use it)
        public async Task DeleteAsync_ShouldOnlyRemoveLink_WhenServiceIsSharedWithOthers()
        {
            // 1. Arrange
            var serviceId = 1;
            var userId = "provider-1";
            var fakeProvider = new Provider(userId, "Provider 1");
            var sharedService = new Service("Shared Service", 150);

            _unitofWorkMock.Setup(u => u.Providers.GetByUserIdWithDetailsAsync(userId))
                           .ReturnsAsync(fakeProvider);
            _unitofWorkMock.Setup(u => u.Services.GetByIdWithImagesAsync(serviceId))
                           .ReturnsAsync(sharedService);
            //Simulate the existence of another provider (returns a non-empty list)
            var otherProviders = new List<ProviderService> { new ProviderService(serviceId, 150) };
            _unitofWorkMock.Setup(u => u.ProviderServices.FindAsync(It.IsAny<Expression<Func<ProviderService, bool>>>()))
                           .ReturnsAsync(otherProviders);
            // act 
            var result = await _serviceService.DeleteAsync(serviceId, userId);
            // assert 
            result.Should().BeTrue();

            _unitofWorkMock.Verify(u => u.Services.DeleteAsync(It.IsAny<Service>()), Times.Never);

            _fileServiceMock.Verify(f => f.DeleteFile(It.IsAny<string>()), Times.Never);
            _unitofWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task GetPagedAsync_ShouldReturnCorrectPagedData_WhenSearchingByTitle()
        {
            // 1. Arrange
            var pagingParams = new PaginationParams
            {
                PageNumber = 1,
                PageSize = 10,
                Search = "cleaning"
            };

            var fakeServices = new List<Service>
            {
                new Service("House Cleaning", 100),
                new Service("Office Cleaning", 200)
            };

            // محاكاة مخرجات الـ Repository (قائمة الخدمات والعدد الإجمالي)
            var totalCount = 2;
            _unitofWorkMock.Setup(u => u.Services.GetPagedAsync(
                pagingParams.PageNumber,
                pagingParams.PageSize,
                It.IsAny<Expression<Func<Service, bool>>>(), // الفلتر
                "Images")) // الـ Include
                .ReturnsAsync((fakeServices, totalCount));

            var readDtos = new List<ServiceReadDto>
    {
        new ServiceReadDto { Title = "House Cleaning" },
        new ServiceReadDto { Title = "Office Cleaning" }
    };

            _mapperMock.Setup(m => m.Map<IEnumerable<ServiceReadDto>>(fakeServices))
                       .Returns(readDtos);

            // 2. Act
            var result = await _serviceService.GetPagedAsync(pagingParams);

            // 3. Assert
            result.Should().NotBeNull();
            result.TotalCount.Should().Be(totalCount);
            result.Items.Should().HaveCount(2);
            result.PageNumber.Should().Be(pagingParams.PageNumber);

            // التأكد أن الـ Repository تم مناداته بالبارامترات الصحيحة
            _unitofWorkMock.Verify(u => u.Services.GetPagedAsync(
                pagingParams.PageNumber,
                pagingParams.PageSize,
                It.IsAny<Expression<Func<Service, bool>>>(),
                "Images"), Times.Once);
        }


    }


}

