namespace LibraryManagement.Services.MappingProfiles
{
    internal sealed class UserMappingProfile : Profile
    {
        public UserMappingProfile() 
        {
            CreateMap<IdentityUserInfo, UserDto>();
        }
    }
}