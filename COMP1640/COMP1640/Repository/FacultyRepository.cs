using COMP1640.Models;
using Humanizer.Localisation;

namespace COMP1640.Repository
{
    public class FacultyRepository : IFacultyRepository
    {
        private readonly UmcsContext _context;

        public FacultyRepository(UmcsContext context)
        {
            _context = context;
        }

        public Faculty Add(Faculty faculty)
        {
            _context.Faculties.Add(faculty);
            _context.SaveChanges();
            return faculty;
        }

        public Faculty Delete(int id)
        {
            Faculty faculty = _context.Faculties.Find(id)!;

            if (faculty != null)
            {
                _context.Faculties.Remove(faculty);
                _context.SaveChanges();
            }

            return faculty!;
        }

        public IEnumerable<Faculty> GetAllFaculties()
        {
            return _context.Faculties.ToList();
        }

        public Faculty GetFaculty(int id)
        {
            return _context.Faculties.Find(id)!;
        }

        public Faculty Update(Faculty faculty)
        {
            _context.Update(faculty);
            _context.SaveChanges();
            return faculty;
        }
    }
}
