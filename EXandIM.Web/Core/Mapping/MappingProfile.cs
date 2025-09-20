using AutoMapper;
using EXandIM.Web.Core.Models;
using EXandIM.Web.Core.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace EXandIM.Web.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Users
            CreateMap<ApplicationUser, UserViewModel>()
               .ForMember(dest => dest.Team, opt => opt.MapFrom(src => src.Team!.Name))
               .ForMember(dest => dest.Circle, opt => opt.MapFrom(src => src.Team!.Circle.Name))
                .ReverseMap();
            CreateMap<UserFormViewModel, ApplicationUser>()
                     .ForMember(dest => dest.NormalizedUserName, opt => opt.MapFrom(src => src.UserName.ToUpper()))
                     .ForMember(dest => dest.password, opt => opt.Ignore())
                    .ReverseMap();


            // Circle
            CreateMap<Circle, CircleViewModel>();
            CreateMap<CircleFormViewModel, Circle>().ReverseMap();
            CreateMap<Circle, SelectListItem>()
               .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));
            CreateMap<ActivityBook, SelectListItem>()
               .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Title));

            // Team
            CreateMap<Team, TeamViewModel>()
                 .ForMember(dest => dest.Circle, opt => opt.MapFrom(src => src.Circle!.Name))
                 .ReverseMap();
            CreateMap<TeamFormViewModel, Team>().ReverseMap();
            CreateMap<Team, SelectListItem>()
               .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));
            // Entity
            CreateMap<Entity, EntityViewModel>();
            CreateMap<EntityFormViewModel, Entity>().ReverseMap();
            CreateMap<Entity, SelectListItem>()
               .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));
            // ReferenceNumber
            CreateMap<ReferenceNumber, ReferenceNumberDetailsViewModel>();
            CreateMap<ReferenceNumber, ReferenceNumberViewModel>();
            CreateMap<ReferenceNumberFormViewModel, ReferenceNumber>().ReverseMap();
            CreateMap<ReferenceNumber, SelectListItem>()
               .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

            // Side Entity
            CreateMap<SideEntity, SideEntityViewModel>();
            CreateMap<SideEntityFormViewModel, SideEntity>().ReverseMap();
            CreateMap<SideEntity, SelectListItem>()
             .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
             .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

            // Sub Entity
            CreateMap<SubEntity, SubEntityViewModel>()
                 .ForMember(dest => dest.Entity, opt => opt.MapFrom(src => src.Entity!.Name))
                .ReverseMap();
            CreateMap<SubEntityFormViewModel, SubEntity>()
                .ReverseMap();
            CreateMap<SubEntity, SelectListItem>()
              .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
              .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));
            // Second Sub Entity
            CreateMap<SecondSubEntity, SecondSubEntityViewModel>()
                 .ForMember(dest => dest.Entity, opt => opt.MapFrom(src => src.Entity!.Name))
                 .ForMember(dest => dest.SubEntity, opt => opt.MapFrom(src => src.SubEntity!.Name))
                .ReverseMap();
            CreateMap<SecondSubEntityFormViewModel, SecondSubEntity>()
                .ReverseMap();
            CreateMap<SecondSubEntity, SelectListItem>()
              .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
              .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

            // Export
            CreateMap<Book, ExportBookViewModel>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User!.FullName))
                .ForMember(dest => dest.Entities, opt => opt.MapFrom(src => src.Entities!.Select(t => t.Name).ToList()))
                .ForMember(dest => dest.SubEntities, opt => opt.MapFrom(src => src.SubEntities!.Select(t => t.Name).ToList()))
                .ForMember(dest => dest.SecondSubEntities, opt => opt.MapFrom(src => src.SecondSubEntities!.Select(t => t.Name).ToList()))
                .ForMember(dest => dest.SideEntity, opt => opt.MapFrom(src => src.SideEntity!.Name))
                .ForMember(dest => dest.Teams, opt => opt.MapFrom(src => src.Teams!.Select(t => t.Name).ToList()))
                .ForMember(dest => dest.Circle, opt => opt.MapFrom(src => src.Teams.FirstOrDefault()!.Circle.Name))
                .ForMember(dest => dest.ExistingFiles, opt => opt.MapFrom(src => src.BookImages))
                .ReverseMap();

            CreateMap<BookFile, BookFileDisplay>()
               .ReverseMap();
            CreateMap<ReadingFile, BookFileDisplay>()
             .ReverseMap();

            CreateMap<ExportBookFormViewModel, Book>()
                .ReverseMap();
            // Import
            CreateMap<Book, ImportBookViewModel>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User!.FullName))
              .ForMember(dest => dest.Entities, opt => opt.MapFrom(src => src.Entities!.Select(t => t.Name).ToList()))
                .ForMember(dest => dest.SubEntities, opt => opt.MapFrom(src => src.SubEntities!.Select(t => t.Name).ToList()))
                .ForMember(dest => dest.SecondSubEntities, opt => opt.MapFrom(src => src.SecondSubEntities!.Select(t => t.Name).ToList()))
                .ForMember(dest => dest.SideEntity, opt => opt.MapFrom(src => src.SideEntity!.Name))
                .ForMember(dest => dest.Teams, opt => opt.MapFrom(src => src.Teams!.Select(t => t.Name).ToList()))
                .ForMember(dest => dest.Circle, opt => opt.MapFrom(src => src.Teams.FirstOrDefault()!.Circle.Name))
                .ForMember(dest => dest.ExistingFiles, opt => opt.MapFrom(src => src.BookImages))
                .ReverseMap();
            CreateMap<ImportBookFormViewModel, Book>()
                .ReverseMap();
            CreateMap<UnAcceptedImportBookFormViewModel, Book>()
                .ReverseMap();
            // Reading
            CreateMap<Reading, ReadingViewModel>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User!.FullName))
                .ForMember(dest => dest.ExistingFiles, opt => opt.MapFrom(src => src.ReadingImages))
                .ForMember(dest => dest.Entities, opt => opt.MapFrom(src => src.Entities!.Select(t => t.Name).ToList()))
                .ForMember(dest => dest.SubEntities, opt => opt.MapFrom(src => src.SubEntities!.Select(t => t.Name).ToList()))
                .ForMember(dest => dest.SecondSubEntities, opt => opt.MapFrom(src => src.SecondSubEntities!.Select(t => t.Name).ToList())).ReverseMap();
            CreateMap<ReadingFormViewModel, Reading>()
                .ReverseMap();



            CreateMap<ActivityBook, ActivityViewModel>()
              .ReverseMap();

            // Item In Activity
            CreateMap<ItemInActivity, ItemInActivityViewModel>()
               .ForMember(dest => dest.Book, opt => opt.MapFrom(src => src.Book!.Title))
               .ForMember(dest => dest.Reading, opt => opt.MapFrom(src => src.Reading!.Title))
               .ForMember(dest => dest.Procedure, opt => opt.MapFrom(src => src.Procedure!))
               .ForMember(dest => dest.ProcedureDate, opt => opt.MapFrom(src => src.ProcedureDate!))
               .ReverseMap();

        }

    }
}