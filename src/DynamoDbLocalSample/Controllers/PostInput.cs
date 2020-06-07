namespace DynamoDbLocalSample.Controllers
{
    public partial class SampleController
    {
        public class PostInput
        {
            public int Id { get; set; }
            public string Title { get; set; }
        }
    }
}