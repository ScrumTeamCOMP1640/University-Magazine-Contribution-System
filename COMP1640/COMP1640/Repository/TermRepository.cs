using COMP1640.Models;

namespace COMP1640.Repository
{
    public class TermRepository : ITermRepository
    {
        private readonly UmcsContext _context;

        public TermRepository(UmcsContext context)
        {
            _context = context;
        }

        public Term Add(Term term)
        {
            _context.Terms.Add(term);
            _context.SaveChanges();
            return term;
        }

        public Term Delete(int id)
        {
            Term term = _context.Terms.Find(id)!;

            if (term != null)
            {
                _context.Terms.Remove(term);
                _context.SaveChanges();
            }

            return term!;
        }

        public IEnumerable<Term> GetAllTerms()
        {
            return _context.Terms.ToList();
        }

        public Term GetTerm(int id)
        {
            return _context.Terms.Find(id)!;
        }

        public Term Update(Term term)
        {
            _context.Update(term);
            _context.SaveChanges();
            return term;
        }
    }
}
