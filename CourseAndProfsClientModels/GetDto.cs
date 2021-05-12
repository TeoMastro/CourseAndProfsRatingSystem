namespace CourseAndProfsClientModels
{
  using System;
  using System.ComponentModel;
  using System.ComponentModel.DataAnnotations;

  public record AddProfessorDto(string FullName, string Mail, string Phone, string Office, string EOffice, string Department);
  
  public record AddCourseDto(string Name, string Type);

}
