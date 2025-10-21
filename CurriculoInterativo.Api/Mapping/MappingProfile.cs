using AutoMapper;
using CurriculoInterativo.Api.DTOs;
using CurriculoInterativo.Api.Models;
using CurriculoInterativo.Api.Entities;
using CurriculoInterativo.Api.DTOs.ExperienceDto;

namespace CurriculoInterativo.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Certification mappings
            CreateMap<Certification, CertificationModel>();
            CreateMap<CertificationModel, Certification>();




            // Contact mappings
            CreateMap<Contact, ContactModel>();
            CreateMap<ContactModel, Contact>();

            // Experience mappings
            CreateMap<Experience, ExperienceModel>();
            CreateMap<ExperienceModel, Experience>();

            // NOVO MAPEAMENTO para a rota leve de Logos
            CreateMap<Experience, ExperienceResponse>();

            // Project mappings
            CreateMap<Project, ProjectModel>();
            CreateMap<ProjectModel, Project>();

            // Responsibility mappings
            CreateMap<Responsibility, ResponsibilityModel>();
            CreateMap<ResponsibilityModel, Responsibility>();

            // Skill mappings
            CreateMap<Skill, SkillModel>();
            CreateMap<SkillModel, Skill>();
        }
    }
}
