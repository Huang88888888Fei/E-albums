﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Threading;

//Cavan & Zach: This class deals with events that use or modify data in the XML(.abm) files
namespace PhotoAlbumViewOfTheGods
{
    /// <summary>
    /// Class to keep track of albums and their data
    /// </summary>
    class XMLInterface
    {
        private List<pictureData> dataList = new List<pictureData>();//Vector containing picture info
        private string directory;
        private string albumFolder;
        private string photoFolder;
        private string albumExtension;
        private string usersDirectory;
        private string _filePath = "";
        private string _currentAlbumName;
        private string _currentUser;

        //Gets filepath
        public string filePath
        {
            get {return _filePath;}
            set {
                _currentAlbumName = System.IO.Path.GetFileName(value);
                _filePath = value; }
        }

        public string CurrentUser
        {
            get { return _currentUser; }
            set
            {
                _currentUser = value;
                albumFolder = usersDirectory + "\\" + _currentUser;
            }
        }

        public string currentAlbum
        {
            get { return _currentAlbumName; }
            set { _currentAlbumName = value; }
        }

        /// <summary>
        /// Constructor that initializes variables used in this class
        /// </summary>
        /// <param name="folderDir">Absolute path</param>
        /// <param name="albumDirectory">Directory where albums are stored for current user</param>
        /// <param name="photoDirectory">Directory where photos are stored</param>
        /// <param name="ext">Album file extension</param>
        /// <param name="currentUser">Current user</param>
        public XMLInterface(string folderDir, string albumDirectory, string photoDirectory, string ext, string currentUser)
        {
            directory = folderDir;
            usersDirectory = albumDirectory;
            albumFolder = albumDirectory + "\\" + currentUser;
            photoFolder = photoDirectory;
            albumExtension = ext;
            _currentUser = currentUser;
        }

        /// <summary>
        /// sets the data from the dataStruct into the dataList vector, returns false if id is too big
        /// </summary>
        /// <param name="dataStruct">Structure containing data to set</param>
        /// <param name="id">Image id to set data of</param>
        /// <returns>True/False whether image is valid and data was set</returns>
        public bool setData(pictureData dataStruct, int id)
        {
            if (id >= dataList.Count)
                return false;
            else
            {
                dataList[id] = dataStruct;
                return true;
            }
        }

        /// <summary>
        /// Copies image to new location
        /// David
        /// </summary>
        /// <param name="from">Original location</param>
        /// <param name="to">New location</param>
        private void copyImage(string from, string to)
        {
            File.Copy(from, to);
        }

        /// <summary>
        /// Function is called when importing a picture, where path is the picture's path
        /// Copies the pic to a new photos folder
        /// Compares pictires if a duplicate name exists
        /// adds the image to the datalist
        /// sets a default description to nothing
        /// Zach & Cavan
        /// </summary>
        /// <param name="path">Path to picture to import</param>
        /// <param name="allImages">List of all current images</param>
        public void addPhoto(string path, ref List<Utilities.AllImagesInfo> allImages)
        {
            int totalImages = allImages.Count;            
            string newPath;
            bool alreadyExists = false;
            string dateAdded = Utilities.getTimeStamp();
            string imageName = Utilities.getNameFromPath(path);
            string calculateMD5 = Utilities.CalculateMD5(path);
            Utilities.AllImagesInfo newImage;
            pictureData image = new pictureData();
            
            newPath = directory + photoFolder + "\\" + imageName + Path.GetExtension(path);
            //Checks to see if a pic with the same name exists, checks if it exists then compare the pics
            try
            {
                for (int i = 0; i < totalImages; i++)
                {
                    if (allImages[i].MD5 == calculateMD5)
                    {
                        alreadyExists = true;
                        newPath = allImages[i].path;
                        break;
                    }
                }

                if (!alreadyExists) //if the md5 was not found
                {
                    if (File.Exists(newPath)) //check to see if a file w/ the same name exists
                    {
                        newPath = Utilities.getAppendName(newPath); //append a number to the end of it
                    }
                    new Thread(() => copyImage(path, newPath)).Start(); //copy that image over to our photos directory in a new thread
                }

                newImage.path = newPath;
                newImage.MD5 = calculateMD5;
                image.description = "";
                image.dateAdded = dateAdded;
                image.dateModified = dateAdded;
                image.MD5 = calculateMD5;
                image.path = newPath;
                image.name = imageName;
                image.id = Utilities.getIdFromInt(dataList.Count);
                allImages.Add(newImage);
                dataList.Add(image);
            }
            catch { }
            
        }

        /// <summary>
        /// Function is used to get picture info from and element in the datalist
        /// The id of the picture is sent and the info for that pic is returned
        /// </summary>
        /// <param name="id">ID of picture to retrieve data of</param>
        /// <returns>Data retrieved</returns>
        public pictureData getData(int id)
        {
            pictureData tempData = new pictureData();
            tempData.name = "";
            tempData.id = "0";
            tempData.path = "";
            tempData.description = "";
            //if invalid id is received
            if (id >= dataList.Count)
                return tempData;
            else
            {
                tempData = dataList[id];
                return tempData;
            }
        }

        /// <summary>
        /// Gets the list of pictures which are used for the list view
        /// Names are retrieved from the datalist
        /// </summary>
        /// <returns>List of pictures in album</returns>
        public string[] getPictureList()
        {
            string[] picList = new string[dataList.Count];
            if (isEmpty())
                return picList;
            else
            {
                for (int i = dataList.Count - 1; i >= 0; i--)
                {
                    picList[i] = dataList[i].name;
                }
                return picList;
            }
        }

        /// <summary>
        /// Returns an array of the albums
        /// </summary>
        /// <returns>List of available albums</returns>
        public string[] getAlbumList()
        {
            return Directory.GetFiles(directory + albumFolder, "*" + albumExtension);
        }

        /// <summary>
        /// Clears datalist vector
        /// </summary>
        public void clearAlbum()
        {
            dataList.Clear();
        }

        /// <summary>
        /// Deletes selected album with the given name, returns true if successful
        /// Cavan & Zach
        /// </summary>
        /// <param name="albumName">Name of album to delete</param>
        /// <returns>True/false as to whether album was successfuly deleted</returns>
        public bool deleteAlbum(string albumName)
        {
            //delete file using directory, albumName
            try
            {
                File.Delete(albumName);
                return true;
            }
            catch
            {
                //if can't access file to delte
                return false;
            }
        }

        /// <summary>
        /// Creates a new album with the user entered name returns true if successful
        /// Zach
        /// </summary>
        /// <param name="albumName">Name of album to create</param>
        /// <returns>Whether album creation was successful</returns>
        public bool createAlbum(string albumName)
        {
            //create file using directory, albumName
            try
            {
                FileStream f = File.Create(directory + albumFolder + "\\" + albumName + albumExtension);
                f.Close();
                //Close Current
                dataList.Clear();
                return true;
            }
            catch
            {
                //if can't create file
                return false;
            }
        }

        /// <summary>
        /// Loads the album information from a .abm(XML format) file and adds information to a query
        /// then, the datalist vector will get each pictures information
        /// Zach
        /// </summary>
        /// <param name="albumName">Name of album to open</param>
        /// <returns>Whether album was successfuly opened</returns>
        public bool loadAlbum(string albumName)
        {
            pictureData PicData = new pictureData();
            //if fails to get or read file
            //return false;
            XDocument xdoc = new XDocument();
            try
            {
                xdoc = XDocument.Load(albumName);
            }catch
            {
                return false;
            }
            //Run query, only header item is the album name. children contains all pciture information
            var Albums = from AlbumInfo in xdoc.Descendants("AlbumInfo")
                       select new
                       {
                           Header = AlbumInfo.Attribute("name").Value,
                           Children = AlbumInfo.Descendants("PictureInfo")
                       };
            //Loop through results and add the info to the datalist for each picture
            foreach (var albumInfo in Albums)
            {
                foreach (var PictureInfo in albumInfo.Children)
                {
                    PicData.id = PictureInfo.Attribute("id").Value;
                    PicData.path = PictureInfo.Attribute("path").Value;
                    PicData.name = PictureInfo.Attribute("name").Value;
                    PicData.description = PictureInfo.Attribute("description").Value;
                    PicData.MD5 = PictureInfo.Attribute("md5").Value;
                    PicData.dateAdded = PictureInfo.Attribute("dateAdded").Value;
                    PicData.dateModified = PictureInfo.Attribute("dateModified").Value;
                    PicData.albumPath = albumName;
                    dataList.Add(PicData);
                }
            }
            //read and load xml data into dataList
            _filePath = albumName;
            _currentAlbumName = albumName;

            return true;


        }

        /// <summary>
        /// Save function that queries the datalist and writes it in XML format
        /// Zach
        /// </summary>
        /// <returns>Whether save was successful or not</returns>
        public bool saveAlbum()
        {
            //loop and load each element into pictureData struct, then 
            //write values in xml
            string saveTo = directory + albumFolder + "\\" + _currentAlbumName;
            if (_currentAlbumName != "")
            {
                try
                {
                    XElement xmlSave = new XElement("Album",
                        new XElement("AlbumInfo",
                            new XAttribute("name", saveTo),
                            from picInfo in dataList
                            select new XElement("PictureInfo",
                                        new XAttribute("id", picInfo.id),
                                        new XAttribute("path", picInfo.path),
                                        new XAttribute("name", picInfo.name),
                                        new XAttribute("description", picInfo.description),
                                        new XAttribute("md5", picInfo.MD5),
                                        new XAttribute("dateAdded", picInfo.dateAdded),
                                        new XAttribute("dateModified",picInfo.dateModified)
                            )
                        ));
                    //_filepath is the albums name
                    xmlSave.Save(saveTo);
                }
                catch (SystemException e)
                {
                    MessageBox.Show("Error: "+e.Message);
                    return false;
                }                
            }
            //Still return true if there was nothing to save
            return true;
        }

        /// <summary>
        /// Returns current xml data in a vector of struct pictureData
        /// </summary>
        /// <returns>List of all pictures</returns>
        public List<pictureData> getDataList()
        {
            return dataList;
        }
    
        /// <summary>
        /// Returns true if no data is loaded into the list
        /// </summary>
        /// <returns>Whether list is empty or not</returns>
        public bool isEmpty()
        {
            if(dataList.Count == 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Parameter is a string value of what thumbnail was clicked on
        /// Removes picture from panel; gets the datalist count and removes the 
        /// Curerntly selected picture, the vector should update itself when an
        /// element is removed
        /// Zach
        /// </summary>
        /// <param name="id">ID of picture to remove</param>
        public void RemovePic(string id)
        {
            try
            {
                int numofpics = dataList.Count - 1;
                int IDvalue;
                int CurrentID = Convert.ToInt32(id);
                pictureData Removing;
                dataList.RemoveAt(CurrentID);
                while (CurrentID != numofpics)
                {
                    Removing = dataList[CurrentID];
                    IDvalue = Convert.ToInt32(Removing.id) - 1;
                    Removing.id = Utilities.getIdFromInt(IDvalue);
                    dataList[CurrentID] = Removing;
                    CurrentID++;
                }
            }
            catch { }
        }
    
    }
}
