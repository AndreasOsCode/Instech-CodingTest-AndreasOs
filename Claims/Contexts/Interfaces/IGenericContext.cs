namespace Claims.Contexts.Interfaces;

public interface IGenericContext<T>
{
    public Task<IEnumerable<T>> GetItemsAsync();

    public Task<T?> GetItemAsync(string id);

    public Task AddItemAsync(T item);

    public Task DeleteItemAsync(string id);
}