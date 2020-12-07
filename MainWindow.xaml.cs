//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Web;

namespace Lab6RestClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HttpClient client;

        public MainWindow()
        {
            InitializeComponent();
            client = new HttpClient();          
            client.BaseAddress = new Uri("http://localhost:57667/");
        }
        private async void GetPeople()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var streamTask = client.GetStreamAsync("api/people");
            var people = await JsonSerializer.DeserializeAsync<List<Person>>(await streamTask);
            PeopleListBox.ItemsSource = people;
        }

        private void GetPeopleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GetPeople();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void GetPersonButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var streamTask = client.GetStreamAsync("api/people/"+IdTB.Text);
                var person = await JsonSerializer.DeserializeAsync<Person>(await streamTask);
                List<Person> people = new List<Person> { person };
                PeopleListBox.ItemsSource = people;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void DelPersonButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PeopleListBox.SelectedItem != null)
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var responseMessage = await client.DeleteAsync("api/people/" + (PeopleListBox.SelectedItem as Person).Id);
                    if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        MessageBox.Show("Pomyślnie usunięto " + (PeopleListBox.SelectedItem as Person).ToString(), "OK", MessageBoxButton.OK, MessageBoxImage.Information);
                        GetPeople();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void AddPersonButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(int.TryParse(YearTB.Text, out int n))
                {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        Person person = new Person();
                        person.Name = NameTB.Text;
                        person.Surname = SurnameTB.Text;
                        person.City = CityTB.Text;
                        person.Year = n;

                        var json = JsonSerializer.Serialize(person);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var responseMessage = await client.PostAsync("api/people?city="+cityCheckBox.IsChecked.ToString(), content);
                        if (responseMessage.StatusCode == System.Net.HttpStatusCode.Created)
                        {
                            MessageBox.Show("Pomyślnie dodano " + person.ToString(), "OK", MessageBoxButton.OK, MessageBoxImage.Information);
                            GetPeople();
                        }
                        else if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            MessageBox.Show(await responseMessage.Content.ReadAsStringAsync(),"OK", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                }
                else
                {                     
                    MessageBox.Show("Niepoprawna wartość w polu Rok","",MessageBoxButton.OK,MessageBoxImage.Error);                    
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void EditPersonButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(YearTB.Text, out int n))
                {
                    if(PeopleListBox.SelectedItem != null)
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        Person person = new Person();
                        person.Name = NameTB.Text;
                        person.Surname = SurnameTB.Text;
                        person.City = CityTB.Text;
                        person.Year = n;

                        var json = JsonSerializer.Serialize(person);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var responseMessage = await client.PutAsync("api/people/" + (PeopleListBox.SelectedItem as Person).Id, content);
                        if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            MessageBox.Show("Pomyślnie zmieniono " + (PeopleListBox.SelectedItem as Person).ToString(), "OK", MessageBoxButton.OK, MessageBoxImage.Information);
                            GetPeople();
                        }
                    }    
                }
                else
                {
                    MessageBox.Show("Niepoprawna wartość w polu Rok", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void FindPersonButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var query = HttpUtility.ParseQueryString(string.Empty);
                query["name"] = NameTB.Text;
                query["surname"] = SurnameTB.Text;
                query["city"] = CityTB.Text;
                query["year"] = YearTB.Text;
                query["lowercase"] = lowercaseCheckBox.IsChecked.ToString();
                query["contains"] = containsCheckBox.IsChecked.ToString();
                string queryString = query.ToString();

                var streamTask = client.GetStreamAsync("api/people/find?" + queryString);
                var people = await JsonSerializer.DeserializeAsync<List<Person>>(await streamTask);
                PeopleListBox.ItemsSource = people;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
