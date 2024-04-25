namespace Claims.Services.Interfaces;

public interface IGenericDbService<T>
{
    public Task<IEnumerable<T>> GetItemsAsync();

    public Task<T?> GetItemAsync(string id);

    public Task<T> AddItemAsync(T item);
    
    public Task DeleteItemAsync(string id);
}