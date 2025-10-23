using CMCS.Web.Controllers;
using CMCS.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace CMCS.Tests
{
    public class ClaimsControllerTests
    {
        private ApplicationDbContext GetInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDB_" + System.Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private IConfiguration GetConfig()
        {
            var data = new Dictionary<string, string>
            {
                {"UploadSettings:MaxFileBytes","5242880"},
                {"UploadSettings:AllowedExtensions:0",".pdf"},
                {"UploadSettings:AllowedExtensions:1",".docx"},
                {"UploadSettings:AllowedExtensions:2",".xlsx"}
            };
            return new ConfigurationBuilder().AddInMemoryCollection(data).Build();
        }

        [Fact]
        public async Task Create_Post_CreatesClaim()
        {
            var db = GetInMemoryDb();
            var envMock = new Mock<IWebHostEnvironment>();
            envMock.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());
            var config = GetConfig();

            var controller = new ClaimsController(db, envMock.Object, config);

            var claim = new Claim
            {
                LecturerName = "Test Lecturer",
                Month = "September 2025",
                TotalHours = 5,
                HourlyRate = 100
            };

            var result = await controller.Create(claim, new List<IFormFile>());

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Single(db.Claims);
        }

        [Fact]
        public async Task Approve_ChangesStatusToApproved()
        {
            var db = GetInMemoryDb();
            var envMock = new Mock<IWebHostEnvironment>();
            envMock.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());
            var config = GetConfig();

            db.Claims.Add(new Claim { LecturerName = "A", Month = "M", TotalHours = 1, HourlyRate = 10 });
            await db.SaveChangesAsync();

            var controller = new ClaimsController(db, envMock.Object, config);

            var claim = await db.Claims.FirstAsync();
            var res = await controller.Approve(claim.Id);

            var refreshed = await db.Claims.FindAsync(claim.Id);
            Assert.Equal(ClaimStatus.Approved, refreshed.Status);
        }
    }
}
