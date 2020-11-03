using Aplicacion.Cursos;
using AutoMapper;
using Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aplicacion
{
    //Realizo la configuracion de Mapper
    public class MappingProfile : Profile //Profile es de AutoMapper
    {
        //Manejamos el mapeo entre las entidades y entidadesDTO
        public MappingProfile()
        {
            //Hago el mapeo de entidad Curso hacia CursoDto
            CreateMap<Curso, CursoDto>()
                //Hago un mapeo manual indicando como se debe mapear la lista de instructores de CursoDto
                .ForMember(x => x.Instructores, y => y.MapFrom(z => z.InstructoresLink.Select(a => a.Instructor).ToList()))
                .ForMember(x => x.Comentarios, y => y.MapFrom(z => z.ComentarioLista)) //Lleno la data de comentarios al DTO
                .ForMember(x => x.Precio, y => y.MapFrom(y => y.PrecioPromocion)); //Lleno la data de precio al DTO
            
            //Hago el mapeo de entidad CursoInstructor hacia CursoInstructorDto
            CreateMap<CursoInstructor, CursoInstructorDto>();
            CreateMap<Instructor, InstructorDto>();
            CreateMap<Comentario, ComentarioDto>();
            CreateMap<Precio, PrecioDto>();
        }
    }
}
