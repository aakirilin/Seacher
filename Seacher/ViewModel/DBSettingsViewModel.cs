using Seacher.Models;

namespace Seacher.ViewModel
{
    public class DBSettingsViewModel : BaseViewModel
    {
        private DBSettings model;

        public string Title 
        { 
            get => model.Title; 
            set => SetProperty(ref model.Title, value); 
        }
        public string Description
        {
            get => model.Description;
            set => SetProperty(ref model.Description, value);
        }
        public string ConnectionString
        {
            get => model.ConnectionString;
            set => SetProperty(ref model.ConnectionString, value);
        }

        public DBSettingsViewModel()
        {
            model = new DBSettings();
        }
    }
}
