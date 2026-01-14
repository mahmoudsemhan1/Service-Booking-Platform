using Application.DTOs.Service;
using Domain.Models;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServiceBooking.UnitTests.IntegrationTests.Helpers;
using System.Net.Http.Json;
using Xunit;

namespace ServiceBooking.UnitTests.IntegrationTests.Controllers
{
    public class ServicesControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;


        public ServicesControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            // create client the will be the browser or postman
            _client = factory.CreateClient();
        }
        [Fact]
        public async Task GetAll_ShouldReturnOkAndListOfServices()
        {
            // Arrange

            //act
            var response = await _client.GetAsync("/api/services");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var services = await response.Content.ReadFromJsonAsync<object>();
            services.Should().NotBeNull();
        }

        [Fact]
        public async Task GetById_ShouldReturnService_WhenServiceExists()
        {

            // 1. Arrange 
            var serviceId = 55;
            var testTitle = "VIP Home Cleaning";
            var testPrice = 500m;
            //
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                // sure that no data is exisit (old data)
                db.Services.RemoveRange(db.Services);

                var testService = new Domain.Models.Service(testTitle, testPrice);
                db.Services.Add(testService);
                await db.SaveChangesAsync();

                serviceId = testService.Id;
            }

            //act 
            var response = await _client.GetAsync($"/api/services/{serviceId}");

            //asssert 
            var result = await response.Content.ReadFromJsonAsync<ServiceReadDto>();

            result.Should().NotBeNull();
            result.Id.Should().Be(serviceId);
            result.Title.Should().Be(testTitle);

        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            // by using Id not exisit 
            //act
            var response = await _client.GetAsync("/api/services/9999");

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        }

        [Fact]
        public async Task CreateService_ShouldReturnCreated_WhenValid()
        {
            //Arrange
            // 1. Arrange
            var testUserId = "TestProviderGuid"; // نفس الـ ID اللي في الـ Handler

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // هل عندك جدول اسمه Providers؟ لازم نضيف فيه سجل مربوط بالـ User
                // هفترض إن الكيان اسمه Provider وعنده خاصية UserId
                var existingProvider = await db.Providers.FirstOrDefaultAsync(p => p.UserId == testUserId);
                if (existingProvider == null)
                {
                    // ضيف Provider جديد عشان الـ Service تلاقيه
                    db.Providers.Add(new Domain.Models.Provider(testUserId, "Test Provider", "ay hagaaaa"));
                    await db.SaveChangesAsync();
                }
            }

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("TestScheme");

            var content = new MultipartFormDataContent();
            content.Add(new StringContent("New Plumbing Service"), "Title");
            content.Add(new StringContent("250"), "Price");
            content.Add(new StringContent("Description for plumbing"), "Description");
            content.Add(new StringContent("60"), "DurationMinutes");
            content.Add(new StringContent("true"), "IsPrimaryStatus");


            // image
            var fileContent = new ByteArrayContent(new byte[] { 0x1, 0x2, 0x3 });
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            content.Add(fileContent, "ImageFiles", "test.jpg");

            //act 
            var response = await _client.PostAsync("/api/services", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetail = await response.Content.ReadAsStringAsync();
                throw new Exception($"Test failed with status {response.StatusCode}. Error: {errorDetail}");
            }
            // assert 
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

            var result = await response.Content.ReadFromJsonAsync<ServiceReadDto>();

            result.Title.Should().Be("New Plumbing Service");


        }

        [Fact]
        public async Task UpdateService_ShouldReturnNoContent_WhenValid()
        {
            // 1. Arrange: تجهيز البيانات وربطها ببعض
            var testUserId = "TestProviderGuid";
            int existingServiceId;

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // أ. تجهيز الـ Provider
                var provider = await db.Providers
                    .Include(p=>p.ProviderServices) // بنعمل Include للـ private field
                    .FirstOrDefaultAsync(p => p.UserId == testUserId);

                if (provider == null)
                {
                    provider = new Provider(testUserId, "Perfect Cleaning Co", "Downtown Cairo");
                    db.Providers.Add(provider);
                    await db.SaveChangesAsync();
                }

                // ب. إنشاء الخدمة العامة (عشان يكون ليها وجود في السيستم)
                var service = new Service("Deep Cleaning", 200m);
                db.Services.Add(service);
                await db.SaveChangesAsync();
                existingServiceId = service.Id;

                // ج. الررررربط باستخدام الـ Domain Method بتاعتك
                // دي الميثود اللي إنت معرفها في الـ Provider كلاس
                provider.AddService(existingServiceId, 200m);

                await db.SaveChangesAsync();
            }
                _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("TestScheme");

            // ج. تجهيز الـ DTO الجديد (Update)
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(existingServiceId.ToString()), "Id");
            content.Add(new StringContent("Deep Cleaning V2"), "Title");
            content.Add(new StringContent("450"), "Price"); // السعر الجديد اللي هيتعدل
            content.Add(new StringContent("120"), "DurationMinutes");
            content.Add(new StringContent("true"), "IsPrimaryStatus");

            // صورة جديدة للتعديل
            var fileContent = new ByteArrayContent(new byte[] { 0x5, 0x6 });
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            content.Add(fileContent, "ImageFiles", "updated_image.jpg");

            //act 
            var response = await _client.PutAsync($"/api/services/{existingServiceId}", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Update failed: {error}");
            }

            //assert 
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
            //
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var link = await db.Set<ProviderService>()
                    .FirstOrDefaultAsync(ps => ps.ServiceId == existingServiceId);

                link.Should().NotBeNull();
                link!.Price.Should().Be(450m);
            }

        }

        [Fact]
        public async Task DeleteService_ShouldReturnNoContent_WhenServiceExistsAndUserIsOwner()
        {
            // 1. Arrange: تجهيز بروفايدر وخدمة وربطهم ببعض
            var testUserId = "TestProviderGuid";
            int serviceIdToDelete;

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // أ. التأكد من وجود الـ Provider
                var provider = await db.Providers
                    .Include(p => p.ProviderServices)
                    .FirstOrDefaultAsync(p => p.UserId == testUserId);

                if (provider == null)
                {
                    provider = new Provider(testUserId, "Maintenance Experts", "Alexandria");
                    db.Providers.Add(provider);
                    await db.SaveChangesAsync();
                }

                // ب. إنشاء الخدمة في الجدول العام للخدمات
                var service = new Service("Temporary Service", 50m);
                db.Services.Add(service);
                await db.SaveChangesAsync();
                serviceIdToDelete = service.Id;

                // ج. ربط الخدمة بالبروفايدر (Ownership)
                // بنستخدم ميثود الـ Domain اللي إنت عاملها
                provider.AddService(serviceIdToDelete, 50m);
                await db.SaveChangesAsync();
            }

            // إعداد التوثيق (Authentication)
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("TestScheme");

            // 2. Act: إرسال طلب المسح
            var response = await _client.DeleteAsync($"/api/services/{serviceIdToDelete}");

            // 3. Assert: التحقق من النجاح
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);

            // التحقق الفعلي من المسح في قاعدة البيانات
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // التأكد إن الربط (Link) اتمسح
                var linkExists = await db.Set<ProviderService>()
                    .AnyAsync(ps => ps.ServiceId == serviceIdToDelete);

                linkExists.Should().BeFalse("The service link should be removed from the provider's list.");

                // ملحوظة: لو اللوجيك عندك بيمسح الخدمة تماماً من السيستم، ممكن تتأكد برضه هنا:
                // var serviceExists = await db.Services.AnyAsync(s => s.Id == serviceIdToDelete);
                // serviceExists.Should().BeFalse();
            }
        }
    }
}
