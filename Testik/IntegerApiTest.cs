using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebApiBD;
using WebApiBD.Models;
using Xunit;
namespace Testik
{
    public class BazaControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public BazaControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public void Get_ReturnsListOfSotrudniki()
        {
            using var client = _factory.CreateClient();
            var response = client.GetAsync("/api/Baza/GET").Result;
            response.EnsureSuccessStatusCode();
            var responseString = response.Content.ReadAsStringAsync().Result;
            var sotrudnikiList = JsonConvert.DeserializeObject<List<Sotrudniki>>(responseString);
            Assert.NotNull(sotrudnikiList);
            Assert.NotEmpty(sotrudnikiList);
        }
        [Fact]
        public void Post_AddsNewSotrudnik()
        {
            using var client = _factory.CreateClient();
            var newSotrudnik = new Sotrudniki
            {
                IDPolzovateliaDlyaAvtorizacii = 22,
                IDRoli = 1,
                Imya = "Новый",
                Familia = "Сотрудник",
                Otchestvo = "Новый",
                NumberPhone = "+7(961)888-19-02",
                Pochta = "Новый",
                SeriaPasporta = "1234",
                NomerPasporta = "123456"
            };

            var content = new StringContent(JsonConvert.SerializeObject(newSotrudnik), Encoding.UTF8, "application/json");
            var response = client.PostAsync("/api/Baza/POST", content).Result;
            response.EnsureSuccessStatusCode();
            var responseString = response.Content.ReadAsStringAsync().Result;
            Assert.Equal("Сотрудник добавлен в базу данных", responseString);
            VerifySotrudnikInDatabase(newSotrudnik.Imya, newSotrudnik.Familia, newSotrudnik.Otchestvo);
        }

        [Fact]
        public void Put_UpdatesExistingSotrudnik()
        {
            using var client = _factory.CreateClient();
            var updatedSotrudnik = new Sotrudniki
            {
                IDSotrydnika = 1,
                IDPolzovateliaDlyaAvtorizacii = 22,
                IDRoli = 1,
                Imya = "Обновленный",
                Familia = "Обновленный",
                Otchestvo = "Обновленный",
                NumberPhone = "+7(961)888-19-02",
                Pochta = "Обновленный",
                SeriaPasporta = "1234",
                NomerPasporta = "123456"
            };

            var content = new StringContent(JsonConvert.SerializeObject(updatedSotrudnik), Encoding.UTF8, "application/json");
            var response = client.PutAsync("/api/Baza/PUT/1", content).Result;
            response.EnsureSuccessStatusCode();
            var responseString = response.Content.ReadAsStringAsync().Result;
            Assert.Equal($"Информация о сотруднике с ID 1 обновлена в базе данных", responseString);
            VerifySotrudnikInDatabase(1, "Обновленный");
        }

        [Fact]
        public void Delete_RemovesExistingSotrudnik()
        {
            using var client = _factory.CreateClient();
            var response = client.DeleteAsync("/api/Baza/DELETE/3048").Result;
            response.EnsureSuccessStatusCode();
            var responseString = response.Content.ReadAsStringAsync().Result;
            Assert.Equal($"Сотрудник с ID 3048 удален из базы данных", responseString);
            VerifySotrudnikNotInDatabase(3048);
        }

        private void VerifySotrudnikInDatabase(int sotrudnikId, string expectedName)
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var sotrudnik = dbContext.Sotrudniki.Find(sotrudnikId);
            Assert.NotNull(sotrudnik);
            Assert.Equal(expectedName, sotrudnik.Imya);
        }

        private void VerifySotrudnikInDatabase(string imya, string familia, string otchestvo)
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var sotrudnik = dbContext.Sotrudniki
                .FirstOrDefault(s => s.Imya == imya && s.Familia == familia && s.Otchestvo == otchestvo);
            Assert.NotNull(sotrudnik);
        }

        private void VerifySotrudnikNotInDatabase(int sotrudnikId)
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var sotrudnik = dbContext.Sotrudniki.Find(sotrudnikId);
            Assert.Null(sotrudnik);
        }
    }
}