using CourseAndProfsClientModels;
using CourseAndProfsClientModels.Dto;
using CourseAndProfsPersistence.Models;
using Kritikos.PureMap;
using Kritikos.PureMap.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseAndProfsClient.Helpers
{
  public static class MappingConfiguration
  {
    public static readonly IPureMapperConfig Mapping = new PureMapperConfig()
      .Map<Professor, ProfessorDto>(mapper => professor => new ProfessorDto() { Id = professor.Id, FullName = professor.FullName, Mail = professor.Mail, Office = professor.Office, EOffice = professor.EOffice, Phone = professor.Phone })
      .Map<ProfessorDto, Professor>(mapper => professorDto => new Professor() { Id = professorDto.Id, FullName = professorDto.FullName, Mail = professorDto.Mail, Office = professorDto.Office, EOffice = professorDto.EOffice, Phone = professorDto.Phone })
      .Map<AddProfessorDto, Professor>(mapper => addProfessor => new Professor() { FullName = addProfessor.FullName, Mail = addProfessor.Mail, Office = addProfessor.Office, EOffice = addProfessor.EOffice, Phone = addProfessor.Phone })
      .Map<Course, CourseDto>(mapper => course => new CourseDto() { Id = course.Id, Name = course.Name })
      .Map<CourseDto, Course>(mapper => courseDto => new Course() { Id = courseDto.Id, Name = courseDto.Name })
      .Map<AddCourseDto, Course>(mapper => addCourse => new Course() { Name=addCourse.Name });
 }
}
