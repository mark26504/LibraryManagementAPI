using LibraryManagement.Shared.Dtos.Books;

namespace LibraryManagement.Services.MappingProfiles
{
    internal sealed class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Book, BookResponse>();
            CreateMap<CreateBookRequest, Book>();
            CreateMap<UpdateBookRequest, Book>();
        }
    }
}
