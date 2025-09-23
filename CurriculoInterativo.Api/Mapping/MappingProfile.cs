using AutoMapper;
using CurriculoInterativo.Api.DTOs;
using CurriculoInterativo.Api.Models;

namespace CurriculoInterativo.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Certification mappings
            CreateMap<Certification, CertificationDto>();
            CreateMap<CertificationDto, Certification>();

            // Contact mappings
            CreateMap<Contact, ContactDto>();
            CreateMap<ContactDto, Contact>();

            // Experience mappings
            CreateMap<Experience, ExperienceDto>();
            CreateMap<ExperienceDto, Experience>();

            // Project mappings
            CreateMap<Project, ProjectDto>();
            CreateMap<ProjectDto, Project>();

            // Responsibility mappings
            CreateMap<Responsibility, ResponsibilityDto>();
            CreateMap<ResponsibilityDto, Responsibility>();

            // Skill mappings
            CreateMap<Skill, SkillDto>();
            CreateMap<SkillDto, Skill>();
        }
    }
}
