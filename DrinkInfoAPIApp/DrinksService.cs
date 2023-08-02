using DrinkInfoAPIApp;
using DrinkInfoAPIApp.Models;
using Newtonsoft.Json;
using RestSharp;
using System.Reflection;
using System.Web;

public class DrinksService
{
    public List<Category> GetCategories()
    {
        var client = new RestClient("http://www.thecocktaildb.com/api/json/v1/1/");
        var request = new RestRequest("list.php?c=list");
        var response = client.ExecuteAsync(request);

        List<Category> categories = new();

        if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
        {
            string rawResponse = response.Result.Content;
            var categoriesFromResponse = JsonConvert.DeserializeObject<Categories>(rawResponse);

            categories = categoriesFromResponse.CategoriesList;
            TableVisualisationEngine.ShowTable(categories, "Categories Menu");
            return categories;
        }
        return categories;

    }

    internal void GetDrink(string drink)
    {
        var client = new RestClient("http://www.thecocktaildb.com/api/json/v1/1/");
        var request = new RestRequest($"lookup.php?i={drink}");
        var response = client.ExecuteAsync(request);

        if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
        {
            string rawResponse = response.Result.Content;
            var drinkDetailFromResponse = JsonConvert.DeserializeObject<DrinkDetailObject>(rawResponse);
            List<DrinkDetail> returnedList = drinkDetailFromResponse.DrinkDetailList;
            DrinkDetail drinkDetail = returnedList[0];
            List<object> prepList = new();
            string formattedName = "";
            foreach (PropertyInfo prop in drinkDetail.GetType().GetProperties())
            {
                if (prop.Name.Contains("str"))
                {
                    formattedName = prop.Name.Substring(3);
                }

                if (!string.IsNullOrEmpty(prop.GetValue(drinkDetail)?.ToString()))
                {
                    prepList.Add(new
                    {
                        Key = formattedName,
                        Value = prop.GetValue(drinkDetail)
                    });
                }
            }
            TableVisualisationEngine.ShowTable(prepList, drinkDetail.strDrink);

        }
    }

    internal List<Drink> GetDrinksByCategory(string category)
    {
        var client = new RestClient("http://www.thecocktaildb.com/api/json/v1/1/");
        var request = new RestRequest($"filter.php?c={HttpUtility.UrlEncode(category)}");

        var response = client.ExecuteAsync(request);

        List<Drink> drinks = new();

        if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
        {
            string rawResponse = response.Result.Content;

            var drinksFromResponse = JsonConvert.DeserializeObject<Drinks>(rawResponse);

            drinks = drinksFromResponse.DrinksList;

            TableVisualisationEngine.ShowTable(drinks, "Drinks Menu");
            return drinks;
        }
        return drinks;

    }


}
