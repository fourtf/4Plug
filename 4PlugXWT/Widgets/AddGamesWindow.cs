using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Xwt;

namespace FPlug.Widgets
{
    public enum GameID
    {
        TeamFortress2,
    }

    public class AddGamesWindow : Dialog
    {
        //DataField<string> libraryNameField = new DataField<string>();
        DataField<string> libraryPathField = new DataField<string>();
        ListStore libraryStore;
        ListView libraryView;

        //DataField<bool> gameEnabledField = new DataField<bool>();
        //DataField<string> gamePathField = new DataField<string>();

        Button addButton;
        Button removeButton;
        Button editButton;

        Button okButton;
        Button cancelButton;

        public AddGamesWindow()
        {
            this.Icon = App.Icon;

            var vBox = new VBox();

            vBox.PackStart(new Label("Select your Steam Game Library."));
            //vBox.PackStart(new Label("If you have other "));

            // LIST
            libraryStore = new ListStore(/*libraryNameField,*/ libraryPathField);
            libraryView = new ListView(libraryStore);

            CellView view;
            //view = new TextCellView(libraryNameField);
            //libraryView.Columns.Add(new ListViewColumn("Game", view));
            view = new TextCellView(libraryPathField);
            libraryView.Columns.Add(new ListViewColumn("Path", view));

            vBox.PackStart(libraryView);

            reloadLibraries();

            libraryView.HeightRequest = 220;
            libraryView.HeadersVisible = false;

            libraryView.SelectionMode = SelectionMode.Single;

            libraryView.SelectionChanged += (s, e) =>
            {
                removeButton.Sensitive = editButton.Sensitive = libraryView.SelectedRows.Length != 0;
            };

            // BUTTONS
            HBox buttonBox = new HBox();

            buttonBox.PackStart(addButton = new Button(" Add new steam library "));
            buttonBox.PackStart(removeButton = new Button(" Remove "));
            buttonBox.PackStart(editButton = new Button(" Edit "));

            addButton.Clicked += (s, e) =>
            {
                string path = showEditDialog();
                if (path != null)
                {
                    int index = libraryStore.AddRow();
                    libraryStore.SetValue(index, libraryPathField, path);
                    libraryView.SelectRow(index);
                }
            };

            removeButton.Sensitive = false;
            removeButton.Clicked += (s, e) =>
            {
                int index = libraryView.SelectedRow;
                libraryStore.RemoveRow(index);
                if (libraryStore.RowCount > index)
                    libraryView.SelectRow(index);
                else if (index > 0)
                    libraryView.SelectRow(index - 1);
            };

            editButton.Sensitive = false;
            editButton.Clicked += (s, e) =>
            {
                string path = showEditDialog();
                if (path != null)
                    libraryStore.SetValue(libraryView.SelectedRow, libraryPathField, showEditDialog());
            };

            buttonBox.PackEnd(cancelButton = new Button(" Cancel "));
            buttonBox.PackEnd(okButton = new Button(" Apply "));

            cancelButton.Clicked += (s, e) =>
            {
                Respond(Command.Cancel);
            };

            okButton.Clicked += (s, e) =>
            {
                XSettings.Games.RemoveNodes();
                List<string> libraries = new List<string>();
                for (int i = 0; i < libraryStore.RowCount; i++)
                {
                    var x = new XElement("library");
                    string path = libraryStore.GetValue(i, libraryPathField);
                    x.SetAttributeValue("path", path);
                    libraries.Add(path);
                    XSettings.Games.Add(x);
                }
                App.ReloadGames(libraries, null);

                Respond(Command.Ok);
            };

            vBox.PackStart(buttonBox);

            // MISC
            Title = "Select your Games";

            Width = 500;
            Content = vBox;
        }

        string showEditDialog()
        {
            //if (App.CustomSelectFolder != null)
            //{
            //    string path;
            //    if (libraryView.SelectedRows.Length != 0 && Directory.Exists(path = libraryStore.GetValue(libraryView.SelectedRow, libraryPathField)))
            //        return App.CustomSelectFolder(path);
            //    return App.CustomSelectFolder(null);
            //}
            //else
            //{
                var dialog = new SelectFolderDialog("Select steam library folder");
                dialog.Multiselect = false;

                {
                    string path;
                    if (libraryView.SelectedRows.Length != 0 && Directory.Exists(path = libraryStore.GetValue(libraryView.SelectedRow, libraryPathField)))
                        dialog.CurrentFolder = path;
                }

                if (dialog.Run(this)) return dialog.Folder;
                else return null;
            //}
        }

        void reloadLibraries()
        {
            foreach (var a in XSettings.Games.Elements("library"))
            {
                var attr = a.Attribute("path");
                if (attr != null)
                {
                    var index = libraryStore.AddRow();
                    libraryStore.SetValue(index, libraryPathField, (string)attr ?? "");
                }
            }
        }
    }
}
