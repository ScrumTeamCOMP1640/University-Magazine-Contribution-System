using COMP1640.Models;

namespace COMP1640.Repository
{
    public interface ITermRepository
    {
        Term Add(Term term);
        Term Update(Term term);
        Term Delete(int id);
        Term GetTerm(int id);
        IEnumerable<Term> GetAllTerms();
    }
}
