
namespace XML_Handler
{
    internal class HeadLetterData(Signatory signatory, Executor executor, District district, List<InvalidsToHead> personList)
    {
        public Signatory signatory = signatory;
        public Executor executor = executor;
        public District district = district;
        public List<InvalidsToHead> personList = personList;
    }
}
