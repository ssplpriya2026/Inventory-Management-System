namespace Inventory_Management_System.Services
{
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }   
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; set; }
    }
}
