using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using QRCodeAttendance.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace QRCodeAttendance.Models
{
    public class StudentAttendance
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
        public DateTime Time { get; set; }
        public ICollection<AttendanceRecord> Attendance { get; set; } = new List<AttendanceRecord>();
    }

    public class AttendanceRecord
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } // Title of the attendance record
        public DateTime DateCreated { get; set; } // Date and time the record was created
        public ICollection<StudentAttendance> Attendees { get; set; } = new List<StudentAttendance>();
    }

    public class EventsAttendeesModel
    {
        [Key]
        [Required]
        public string StudentAttendanceId { get; set; }
        [Key]
        [Required]
        public int AttendanceRecordId { get; set; }
        [Display(Name = "Status")]
        public bool Status { get; set; }


    }

    public class Sample
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string ImageUrl { get; set; }
        [Display(Name = "Student Profile")]
        [EnumDataType(typeof(Sex))]
        public Sex Sex { get; set; } // Added Sex property

        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }

        public Department Department { get; set; }

        public string Course { get; set; }
        [NotMapped]
        public IFormFile? CoverImage { get; set; }
    }
    public enum Sex
    {
        Male,
        Female
    }

    public enum Department
    {
        CIT
    }
    public class SampleCsvMap : ClassMap<Sample>
    {
        public SampleCsvMap()
        {
            Map(m => m.Code).Name("Code");
            Map(m => m.FirstName).Name("FirstName");
            Map(m => m.MiddleName).Name("MiddleName");
            Map(m => m.LastName).Name("LastName");
            Map(m => m.ImageUrl).Name("ImageUrl");
            Map(m => m.Sex).Name("Sex");
            Map(m => m.BirthDate).Name("BirthDate");
            Map(m => m.Department).Name("Department");
            Map(m => m.Course).Name("Course");
        }
    }

    // SampleImportService.cs
    public class SampleImportService
    {
        public IEnumerable<Sample> ParseAndCreateSampleObjects(IFormFile file)
        {
            var samples = new List<Sample>();

            using (var stream = file.OpenReadStream())
            {
                if (file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    // Process CSV file
                    using (var reader = new StreamReader(stream))
                    using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                    {
                        csv.Context.RegisterClassMap<SampleCsvMap>(); // Map CSV columns to Sample object properties
                        samples = csv.GetRecords<Sample>().ToList();
                    }
                }
                else if (file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    // Process Excel file
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0]; // Assuming data is in the first worksheet
                        for (int row = worksheet.Dimension.Start.Row + 1; row <= worksheet.Dimension.End.Row; row++)
                        {
                            samples.Add(new Sample
                            {
                                Code = worksheet.Cells[row, 1].Text,
                                FirstName = worksheet.Cells[row, 2].Text,
                                MiddleName = worksheet.Cells[row, 3].Text,
                                LastName = worksheet.Cells[row, 4].Text,
                                ImageUrl = worksheet.Cells[row, 5].Text,
                                Sex = Enum.Parse<Sex>(worksheet.Cells[row, 6].Text),
                                BirthDate = DateTime.ParseExact(worksheet.Cells[row, 7].Text, "MM/dd/yyyy", CultureInfo.InvariantCulture),
                                Department = Enum.Parse<Department>(worksheet.Cells[row, 8].Text),
                                Course = worksheet.Cells[row, 9].Text
                            });
                        }
                    }
                }
                else
                {
                    // Handle unsupported file formats
                    return null; // You can return null or handle it as appropriate for your application
                }
            }

            return samples;
        }


    }

}


