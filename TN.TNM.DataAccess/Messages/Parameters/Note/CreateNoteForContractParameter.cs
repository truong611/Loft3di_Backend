﻿using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Parameters.Note
{
    public class CreateNoteForContractParameter : BaseParameter
    {
        public Databases.Entities.Note Note { get; set; }
        public List<NoteDocumentEntityModel> ListNoteDocument { get; set; }
        public string ObjType { get; set; }
        public string DescriptionReject { get; set; }
    }
}
