namespace LibraryManagement.Services.MappingProfiles
{
    public class BorrowingProfile : Profile
    {
        public BorrowingProfile()
        {
            CreateMap<BorrowingRecord, BorrowingResponse>()
                .ForMember(dest => dest.Book, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.IsOverdue, 
                           opt => opt.MapFrom(src => src.ReturnedAt == null && src.DueDate < DateTime.UtcNow))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.UserId)); // Fallback if no User nav property exists;
        }
    }
}
