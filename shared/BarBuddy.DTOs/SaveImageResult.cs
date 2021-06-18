using System;
using System.Collections.Generic;
using System.Text;

namespace BarBuddy.DTOs
{
    public  class SaveImageResult
    {
        public static readonly string IMAGEROUTE = "api/image/get/";

        public string ImageUrl { get; set; }
        private string _modelError = string.Empty;
        private string _field = string.Empty;

        public SaveImageResult(string fieldName, string error)
        {
            _modelError = error;
            _field = fieldName;
        }
        public SaveImageResult(string imageUrl)
        {
            ImageUrl = imageUrl;
        }

        public SaveImageResult(int imageId)
        {
            ImageUrl = IMAGEROUTE + imageId;
            ImageId = imageId;
        }
        public int ImageId { get; set; }
        public bool IsError { get { return !string.IsNullOrEmpty(_modelError); } }
        public string Error
        {
            get
            {
                return _modelError;
            }
        }
        public string Field
        {
            get
            {
                return _field;
            }
        }
    }
}

