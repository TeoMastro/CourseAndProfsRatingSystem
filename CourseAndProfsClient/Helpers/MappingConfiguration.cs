using CourseAndProfsClientModels;
using CourseAndProfsClientModels.Dto;
using CourseAndProfsPersistence.Models;
using CourseAndProfsClientModels.Enums;
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
      .Map<Professor, ProfessorDto>(mapper => professor => new ProfessorDto() { Id = professor.Id, FullName = professor.FullName, Mail = professor.Mail, Office = professor.Office, EOffice = professor.EOffice, Phone = professor.Phone, AverageRating = professor.AverageRating , Department =  professor.Department })
      .Map<ProfessorDto, Professor>(mapper => professorDto => new Professor() { Id = professorDto.Id, FullName = professorDto.FullName, Mail = professorDto.Mail, Office = professorDto.Office, EOffice = professorDto.EOffice, Phone = professorDto.Phone })
      .Map<AddProfessorDto, Professor>(mapper => addProfessorDto => new Professor() { FullName = addProfessorDto.FullName, Mail = addProfessorDto.Mail, Office = addProfessorDto.Office, EOffice = addProfessorDto.EOffice, Phone = addProfessorDto.Phone })
      .Map<Course, CourseDto>(mapper => course => new CourseDto() { Id = course.Id, Name = course.Name, Type = course.Type})
      .Map<CourseDto, Course>(mapper => courseDto => new Course() { Id = courseDto.Id, Name = courseDto.Name, Type = courseDto.Type})
      .Map<AddCourseDto, Course>(mapper => addCourseDto => new Course() { Name = addCourseDto.Name, Type = addCourseDto.Type })
      .Map<Review, ReviewDto>(mapper => review => new ReviewDto() {ReviewId = review.Id, CourseId = review.Course.Id, CourseName = review.Course.Name, ProfessorId = review.Professor.Id, ProfessorName = review.Professor.FullName, UsersSubjectScore = review.UsersSubjectScore, Rating = review.Rating, Comments = review.Comments, CreatedAt = review.CreatedAt});

  }
}
