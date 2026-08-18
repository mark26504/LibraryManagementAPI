namespace LibraryManagement.Services.MappingProfiles
{
    internal sealed class AuthorProfile : Profile
    {
        public AuthorProfile()
        {
            CreateMap<CreateAuthorRequest, Author>();
            CreateMap<UpdateAuthorRequest, Author>();

            CreateMap<Author, AuthorResponse>();
        }
    }
}
