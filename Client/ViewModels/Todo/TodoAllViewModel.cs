using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using hub.Client.Services.Web;

namespace hub.Client.ViewModels.Todo
{
    public interface ITodoAllViewModel
    {
        Shared.Models.Todo.Todo PendingTodo { get; }
        List<Shared.Models.Todo.Todo> Todos { get; }

        public Task OnSave();
    }
    
    public class TodoAllViewModel : ITodoAllViewModel
    {
        private readonly AuthedHttpClient _httpClient;

        public TodoAllViewModel(AuthedHttpClient httpClient)
        {
            _httpClient = httpClient;

            PendingTodo = new Shared.Models.Todo.Todo();
        }

        public Shared.Models.Todo.Todo PendingTodo { get; }
        public List<Shared.Models.Todo.Todo> Todos { get; }
        public async Task OnSave()
        {
            await _httpClient.Init();

            var responseMessage = await _httpClient.PutAsJsonAsync("todos", PendingTodo);
            if (responseMessage.IsSuccessStatusCode)
            {
                var todo = await responseMessage.Content.ReadFromJsonAsync<Shared.Models.Todo.Todo>();
            }
        }
    }
}