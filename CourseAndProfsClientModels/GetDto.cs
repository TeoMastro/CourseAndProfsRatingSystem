﻿namespace CourseAndProfsClientModels
{
  using System;
  using System.ComponentModel;
  using System.ComponentModel.DataAnnotations;

  public record AddProfessorDto(string FullName, string Mail, string Phone, string Office, string EOffice);
  
  public record AddCourseDto(string Name);

  public record AddCourseTypeDto(string Name);
}
