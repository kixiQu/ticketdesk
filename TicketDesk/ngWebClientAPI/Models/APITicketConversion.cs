﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TicketDesk.Domain.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace ngWebClientAPI.Models
{
    public class APITicketConversion
    {
        public static Ticket ConvertPOSTTicket(JObject jsonData, string userName)
        {
            FrontEndTicket data = new FrontEndTicket();
            try
            {
                
                data.details = jsonData["details"].ToString();
                data.ticketType = jsonData["ticketType"].ToString();
                
                data.category = jsonData["category"].ToString();
                data.subcategory = jsonData["subcategory"].ToString();
                data.tagList = jsonData["tagList"].ToString();
                data.title = jsonData["title"].ToString();
                

                if (jsonData["priority"] != null )
                {
                    data.priority = jsonData["priority"].ToString();
                }
                if (jsonData["assignedTo"] != null)
                {
                    data.assignedTo = jsonData["assignedTo"].ToString();
                }

                if (jsonData["ownerId"] != null )
                {
                    data.owner = jsonData["ownerId"].ToString();
                } else
                {
                    data.owner = userName;
                }
            }
            catch(Exception ex)
            {
                return null;
            }

            List<TicketTag> tt = new List<TicketTag>();

            List<string> ts = data.tagList.Split(',').ToList();

            foreach(var name in ts)
            {
                tt.Add(new TicketTag() { TagName = name });
            }

            DateTime now = DateTime.Now;

            Ticket ticket = new Ticket
            {
                ProjectId = 1,
                Title = data.title,
                AffectsCustomer = false,
                Category = data.category,
                SubCategory = data.subcategory,
                CreatedBy = data.owner,
                TicketStatus = TicketStatus.Active,
                CreatedDate = now,
                CurrentStatusDate = now,
                CurrentStatusSetBy = data.owner,
                Details = data.details,
                IsHtml = false,
                LastUpdateBy = data.owner,
                LastUpdateDate = now,
                Owner = data.owner,
                Priority = data.priority,
                AssignedTo = data.assignedTo,
                TagList = data.tagList,
                TicketTags = tt,
                TicketType = data.ticketType,
                TicketEvents = new[] { TicketEvent.CreateActivityEvent(data.owner, TicketActivity.Create, null, null, null) },
                SemanticId = now.ToString("yyMMddHHmm"), /*Formatting for semantic numbering.*/
            };

            return ticket;
        }
        public static FrontEndTicket ConvertGETTicket(Ticket ticket)
        {
            FrontEndTicket FETicket = new FrontEndTicket();
            string ticketID;
            /*if (ticket.SemanticId != null)
            {
                ticketID = ticket.SemanticId + ticket.TicketId.ToString();
            }
            else
            {
                ticketID = ticket.CreatedDate.ToString("yyMMddHHmm") + ticket.TicketId.ToString();
            }*/
            ticketID = ticket.CreatedDate.ToString("yyMMddHHmm") + ticket.TicketId.ToString();
            /*uint x; Int64 y;
            if (uint.TryParse(ticketID, out x))
            {
                FETicket.ticketId = (int)x; //gross conversion to string back to int to get around bit shifting
            }
            else
            {
                Int64.TryParse(ticketID, out y);
                FETicket.ticketId = (int)y;
            }*/
            Int64 y = Int64.Parse(ticketID);
            FETicket.ticketId = y;
            FETicket.projectId = ticket.ProjectId;
            FETicket.details = ticket.Details;
            FETicket.priority = ticket.Priority;
            FETicket.ticketType = ticket.TicketType;
            FETicket.category = ticket.Category;
            FETicket.subcategory = ticket.SubCategory; //no subcategory in TicketDesk db, might want to add?
            FETicket.owner = ticket.Owner;
            FETicket.assignedTo = ticket.AssignedTo;
            FETicket.status = ticket.TicketStatus;
            FETicket.tagList = ticket.TagList;
            FETicket.createdDate = ticket.CreatedDate.ToString();
            FETicket.title = ticket.Title;
            FETicket.subcategory = ticket.SubCategory;

            return FETicket;
        }

        public static int ConvertTicketId(Int64 id)
        {
            string sId = id.ToString();
            if(sId.Length < 10)
            {
                //we might be doing some testing with short ints here
                return (int)id;
            }
            //yymmddhhmm
            return int.Parse(sId.Substring(10, sId.Length-10));
        }

        public static FrontEndEvent ConvertEvent(TicketEvent item)
        {
            FrontEndEvent singleEvent = new FrontEndEvent();

            singleEvent.eventId = item.EventId;
            singleEvent.userId = item.EventBy;
            singleEvent.actionText = item.EventDescription;
            singleEvent.date = item.EventDate.ToString();
            singleEvent.comment = item.Comment;
            singleEvent.actionEnum = item.ForActivity.ToString();

            return singleEvent;
        }
    }
    public class FrontEndEvent
    {
        public int eventId { get; set; }
        public string userId { get; set; }
        public string actionText { get; set; }
        public string date { get; set; }
        public string comment { get; set; }
        public string actionEnum { get; set; }

    }
    public class FrontEndTicket
    {
        public Int64 ticketId { get; set; }
        public int projectId { get; set; }
        public string details { get; set; }
        public string priority { get; set; }
        public string ticketType { get; set; }
        public string category { get; set; }
        public string subcategory { get; set; }
        public string owner { get; set; }
        public string assignedTo { get; set; }
        public TicketStatus status { get; set; }
        public string tagList { get; set; }
        public string createdDate { get; set; }
        public string title { get; set; }
    }

    public class JList
    {
        public List<FrontEndTicket> list { get; set; }
    }

    public class EventList
    {
        public List<FrontEndEvent> list { get; set; }
    }
}