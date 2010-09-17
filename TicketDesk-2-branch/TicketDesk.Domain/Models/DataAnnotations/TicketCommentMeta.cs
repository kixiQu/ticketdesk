//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TicketDesk.Domain.Models.DataAnnotations
{
    public partial class TicketCommentMeta
    {
    		
    	[DisplayName("Ticket Id")]
    	[Required]
        public int TicketId { get; set; }
    		
    	[DisplayName("Comment Id")]
    	[Required]
        public int CommentId { get; set; }
    		
    	[DisplayName("Comment Event")]
    	[StringLength(500)]
        public string CommentEvent { get; set; }
    		
    	[DisplayName("Comment")]
        public string Comment { get; set; }
    		
    	[DisplayName("Is Html")]
    	[Required]
        public bool IsHtml { get; set; }
    		
    	[DisplayName("Commented By")]
    	[Required]
    	[StringLength(100)]
        public string CommentedBy { get; set; }
    		
    	[DisplayName("Commented Date")]
    	[Required]
        public System.DateTime CommentedDate { get; set; }
    		
    	[DisplayName("Version")]
    	[Required]
    	[StringLength(8)]
        public byte[] Version { get; set; }
    }
}