namespace LibraryManagement.Services.MappingProfiles
{
    internal sealed class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Book, BookResponse>()
                            .ForMember(d => d.Authors,
                                    o => o.MapFrom(s => s.BookAuthors.Select(ba => ba.Author)));
           
            CreateMap<CreateBookRequest, Book>();
            CreateMap<UpdateBookRequest, Book>()
                .ForMember(d => d.RowVersion, o => o.Ignore());
        }
    }
}
