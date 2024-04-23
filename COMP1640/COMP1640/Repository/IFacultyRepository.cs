using COMP1640.Models;

namespace COMP1640.Repository
{
    public interface IFacultyRepository
    {
        Faculty Add(Faculty faculty);
        Faculty Update(Faculty faculty);
        Faculty Delete(int id);
        Faculty GetFaculty(int id);
        IEnumerable<Faculty> GetAllFaculties();
    }
}
