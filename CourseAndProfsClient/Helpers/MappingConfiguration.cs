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
      .Map<Professor, ProfessorDto>(mapper => professor => new ProfessorDto() { Id = professor.Id, FullName = professor.FullName, Mail = professor.Mail, Office = professor.Office, EOffice = professor.EOffice, Phone = professor.Phone, AverageRating = professor.AverageRating , Department = professor.Department })
      .Map<ProfessorDto, Professor>(mapper => professorDto => new Professor() { Id = professorDto.Id, FullName = professorDto.FullName, Mail = professorDto.Mail, Office = professorDto.Office, EOffice = professorDto.EOffice, Phone = professorDto.Phone })
      .Map<AddProfessorDto, Professor>(mapper => addProfessorDto => new Professor() { FullName = addProfessorDto.FullName, Mail = addProfessorDto.Mail, Office = addProfessorDto.Office, EOffice = addProfessorDto.EOffice, Phone = addProfessorDto.Phone })
      .Map<Course, CourseDto>(mapper => course => new CourseDto() { Id = course.Id, Name = course.Name })
      .Map<CourseDto, Course>(mapper => courseDto => new Course() { Id = courseDto.Id, Name = courseDto.Name })
      .Map<AddCourseDto, Course>(mapper => addCourseDto => new Course() { Name=addCourseDto.Name })
      .Map<CourseType, CourseTypeDto>(mapper => courseType => new CourseTypeDto() { Id = courseType.Id, Name = courseType.Name })
      .Map<CourseTypeDto, CourseType>(mapper => courseTypeDto => new CourseType() { Id = courseTypeDto.Id, Name = courseTypeDto.Name })
      .Map<AddCourseTypeDto, CourseType>(mapper => addCourseTypeDto => new CourseType() { Name = addCourseTypeDto.Name })
      .Map<Department, DepartmentDto>(mapper => department => new DepartmentDto() { Id = department.Id, Name = department.Name })
      .Map<DepartmentDto, Department>(mapper => departmentDto => new Department() { Id = departmentDto.Id, Name = departmentDto.Name })
      .Map<AddDepartmentDto, Department>(mapper => addDepartmentDto => new Department() { Name = addDepartmentDto.Name })
      .Map<Review, ReviewDto>(mapper => review => new ReviewDto() {ReviewId = review.Id, CourseId = review.Course.Id, CourseName = review.Course.Name, ProfessorId = review.Professor.Id, ProfessorName = review.Professor.FullName, UsersSubjectScore = review.UsersSubjectScore, Rating = review.Rating, Comments = review.Comments, CreatedAt = review.CreatedAt});

  }
}
