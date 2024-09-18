using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Text;
using WebApiBD.Models;
namespace WebApiBD.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BazaController : ControllerBase
    {
        public readonly ApplicationDbContext _context;
        

        public BazaController(ApplicationDbContext context)
        {
            _context = context;
       
        }
        [HttpGet]
        public IActionResult GET()
        {
            try
            {
                var sotrudnikiList = _context.Sotrudniki.ToList();
                 
                return Ok(sotrudnikiList);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public IActionResult POST([FromBody] Sotrudniki sotrudnik)
        {
            try
            {
                _context.Sotrudniki.Add(sotrudnik);
                _context.SaveChanges();
                return Ok("Сотрудник добавлен в базу данных");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPut("{id}")]
        public IActionResult PUT(int id, [FromBody] Sotrudniki sotrudnik)
        {
            try
            {
                var existingSotrudnik = _context.Sotrudniki.FirstOrDefault(s => s.IDSotrydnika == id);
                if (existingSotrudnik == null)
                {
                    return NotFound($"Сотрудник с ID {id} не найден");
                }
                existingSotrudnik.IDPolzovateliaDlyaAvtorizacii = sotrudnik.IDPolzovateliaDlyaAvtorizacii;
                existingSotrudnik.IDRoli = sotrudnik.IDRoli;
                existingSotrudnik.Imya = sotrudnik.Imya;
                existingSotrudnik.Familia = sotrudnik.Familia;
                existingSotrudnik.Otchestvo = sotrudnik.Otchestvo;
                existingSotrudnik.NumberPhone = sotrudnik.NumberPhone;
                existingSotrudnik.Pochta = sotrudnik.Pochta;
                existingSotrudnik.SeriaPasporta = sotrudnik.SeriaPasporta;
                existingSotrudnik.NomerPasporta = sotrudnik.NomerPasporta;
                _context.SaveChanges();
                return Ok($"Информация о сотруднике с ID {id} обновлена в базе данных");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public IActionResult DELETE(int id)
        {
            try
            {
                var existingSotrudnik = _context.Sotrudniki.FirstOrDefault(s => s.IDSotrydnika == id);
                if (existingSotrudnik == null)
                {
                    return NotFound($"Сотрудник с ID {id} не найден");
                }
                _context.Sotrudniki.Remove(existingSotrudnik);
                _context.SaveChanges();
                return Ok($"Сотрудник с ID {id} удален из базы данных");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
       
    }
}