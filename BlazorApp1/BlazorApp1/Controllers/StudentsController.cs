﻿using BlazorApp1.Data;
using BlazorApp1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class StudentsController : ControllerBase
	{
		private readonly LibraryDB _context;

		public StudentsController(LibraryDB context)
		{
			_context = context;
		}

		// GET: api/ProductCategories
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Student>>> GetStudent()
		{
			return await _context.Students.Include(c => c.Books).ToListAsync();
		}

		// GET: api/ProductCategories/5
		[HttpGet("{id}")]
		public async Task<ActionResult<Student>> GetStudent(int id)
		{
			var Student = await _context.Students.Include(c => c.Books).FirstOrDefaultAsync(c => c.StudentId == id);

			if (Student == null)
			{
				return NotFound();
			}

			return Student;
		}

		// PUT: api/ProductCategories/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<IActionResult> PutStudent(int id, Student Student)
		{
			if (id != Student.StudentId)
			{
				return BadRequest();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(Student);

					var itemsIdList = Student.Books.Select(i => i.Id).ToList();

					var delItems = await _context.Books.Where(i => i.StudentId == id).Where(i => !itemsIdList.Contains(i.Id)).ToListAsync();

					_context.Books.RemoveRange(delItems);


					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!StudentExists(id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}

				return NoContent();
			}
			return BadRequest(ModelState);

		}

		// POST: api/ProductCategories
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<Student>> PostStudent(Student Student)
		{
			if (ModelState.IsValid)
			{

				_context.Students.Add(Student);
				await _context.SaveChangesAsync();

				return CreatedAtAction("GetStudent", new { id = Student.StudentId }, Student);
			}

			return BadRequest(ModelState);


		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteStudent(int id)
		{
			try
			{
				var student = await _context.Students.FindAsync(id);

				if (student == null)
				{
					return NotFound("Student not found.");
				}
				var relatedBooks = _context.Books.Where(b => b.StudentId == id);
				_context.Books.RemoveRange(relatedBooks);
				_context.Students.Remove(student);
				await _context.SaveChangesAsync();

				return NoContent();
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting student: {ex.Message}");
			}
		}

		private bool StudentExists(int id)
		{
			return _context.Students.Any(e => e.StudentId == id);
		}
	}
	
}
