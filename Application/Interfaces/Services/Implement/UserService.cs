//using Application.DTOs.Identity;
//using Application.DTOs.User;
//using Application.Interfaces.Services.IUserService;
//using AutoMapper;
//using Domain.Interfaces.UnitofWork;
//using Domain.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Application.Interfaces.Services.Implement
//{
//    public class UserService : IUSerService
//    {
//        private readonly usermanager
//        private readonly IUnitofWork _unitofWork;
//        private readonly IMapper _mapper;
//        public UserService(IUnitofWork unitofWork, IMapper mapper)
//        {
//            _unitofWork = unitofWork;
//            this._mapper = mapper;
//        }
//        public async Task<IEnumerable<UserReadDto>> GetAllAsync()
//        {
//           var users=await _unitofWork.Users.GetAllAsync();

//            return _mapper.Map<IEnumerable<UserReadDto>>(users);
            
//        }

//        public async Task<UserReadDto?> GetByIdAsync(string id)
//        {
//            var user= await _unitofWork.Users.FindOneAsync(u=>u.Id==id) ;

//            if (user == null) return null;
//            return  _mapper.Map<UserReadDto>(user);


//        }

//        public async Task<UserReadDto> CreateAsync(UserCreateDto dto)
//        {
//           //email Check 
//           var exists= (await _unitofWork.Users.FindAsync(u=>u.Email==dto.Email)).Any();
//            if (exists) throw new Exception("Email already in use.");

//            var user= _mapper.Map<User>(dto);

//            if (string.IsNullOrEmpty(user.Id))
//            {
//                user.Id = Guid.NewGuid().ToString();
//            }

//            await _unitofWork.Users.AddAsync(user);
//            await _unitofWork.SaveAsync();

//            return _mapper.Map<UserReadDto>(user);


//        }
//        public async Task<bool> UpdateAsync(string id, AppUserUpdateDto dto)
//        {
//            var user = await _unitofWork.Users.FindOneAsync(u => u.Id == id);

//            if (user == null) return false;

//            _mapper.Map(dto,user);
//            await _unitofWork.Users.UpdateAsync(user);
//            await _unitofWork.SaveAsync();

//            return true;
//        }

//        public async Task<bool> DeleteAsync(string id)
//        {
//            var user = await _unitofWork.Users.FindOneAsync(u => u.Id == id);

//            if (user == null) return false;

//            await _unitofWork.Users.DeleteAsync(user);
//            await _unitofWork.SaveAsync();
//            return true;

//        }


//    }
//}
