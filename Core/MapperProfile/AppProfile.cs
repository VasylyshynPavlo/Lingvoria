using AutoMapper;
using Core.Models;

namespace Core.MapperProfile;

public class AppProfile : Profile
{
    public AppProfile()
    {
        CreateMap<Task<Result>, Result>().ReverseMap();
        CreateMap<Task<WordModel>, WordModel>().ReverseMap();
        CreateMap<Task<ExampleModel>, ExampleModel>().ReverseMap();
        
        // CreateMap<Data.Entities.WordsCollectionModel, Core.Models.WordsCollectionModel>();
        // CreateMap<Data.Entities.ExampleModel, Core.Models.ExampleModel>();
        // CreateMap<Data.Entities.UserModel, Core.Models.UserModel>();
        // CreateMap<Data.Entities.WordModel, Core.Models.WordModel>();
        // CreateMap<Core.Models.WordsCollectionModel, Data.Entities.WordsCollectionModel>();
        // CreateMap<Core.Models.ExampleModel, Data.Entities.ExampleModel>();
        // CreateMap<Core.Models.UserModel, Data.Entities.UserModel>();
        // CreateMap<Core.Models.WordModel, Data.Entities.WordModel>();
    }
}