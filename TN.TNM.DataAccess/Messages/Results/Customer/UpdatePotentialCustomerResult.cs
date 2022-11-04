﻿using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Document;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class UpdatePotentialCustomerResult: BaseResult
    {
        public List<DataAccess.Models.Note.NoteEntityModel> ListNote { get; set; }
        public List<FileInFolderEntityModel> ListFileByPotentialCustomer { get; set; }
        public List<LinkOfDocumentEntityModel> ListLinkOfDocument { get; set; }
        public List<EmployeeEntityModel> ListEmpTakeCare { get; set; }
        public List<EmployeeEntityModel> ListPersonalInChange { get; set; }
    }
}
