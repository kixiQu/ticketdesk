﻿// TicketDesk - Attribution notice
// Contributor(s):
//
//      Stephen Redd (https://github.com/stephenredd)
//
// This file is distributed under the terms of the Microsoft Public 
// License (Ms-PL). See http://opensource.org/licenses/MS-PL
// for the complete terms of use. 
//
// For any distribution that contains code from this file, this notice of 
// attribution must remain intact, and a copy of the license must be 
// provided to the recipient.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ganss.XSS;
using TicketDesk.IO;
using TicketDesk.Search.Common;
using ngWebClientAPI;
using ngWebClientAPI.Models;
//using TicketDesk.Web.Identity;
//using TicketDesk.Web.Identity.Model;
using TicketDesk.Localization.Models;


namespace TicketDesk.Domain.Model
{
    public static class TicketExtensions
    {
        public static IEnumerable<SearchIndexItem> ToSeachIndexItems(this IEnumerable<Ticket> tickets)
        {
            return tickets.Select(t => new SearchIndexItem
            {
                Id = t.TicketId,
                ProjectId = t.ProjectId,
                Title = t.Title,
                Details = t.Details,
                Status = t.TicketStatus.ToString(),
                LastUpdateDate = t.LastUpdateDate,
                Tags = string.IsNullOrEmpty(t.TagList) ? new string[] { } : t.TagList.Split(','),
                //not null comments only, otherwise we end up indexing empty array item, or blowing up azure required field
                Events = t.TicketEvents.Where(c => !string.IsNullOrEmpty(c.Comment)).Select(c => c.Comment).ToArray()
            });
        }

        public static HtmlString HtmlDetails(this Ticket ticket)
        {
           
            var content = (ticket.IsHtml) ? ticket.Details : ticket.Details.HtmlFromMarkdown();
            var san = new HtmlSanitizer();
            return new HtmlString(san.Sanitize(content));
        }

        public static SelectList GetPriorityList(this Ticket ticket)
        {
            var context = DependencyResolver.Current.GetService<TdDomainContext>();
            return context.TicketDeskSettings.GetPriorityList(true, ticket.Priority);
        }

        public static SelectList GetCategoryList(this Ticket ticket)
        {
            var context = DependencyResolver.Current.GetService<TdDomainContext>();
            return context.TicketDeskSettings.GetCategoryList(true, ticket.Category);
        }

        public static SelectList GetTicketTypeList(this Ticket ticket)
        {
            var context = DependencyResolver.Current.GetService<TdDomainContext>();
            return context.TicketDeskSettings.GetTicketTypeList(true, ticket.TicketType);
        }

        public static SelectList GetProjectList(this Ticket ticket, int? selectedProject)
        {
            var context = DependencyResolver.Current.GetService<TdDomainContext>();
            return context.Projects.OrderBy(p => p.ProjectName)
                .ToSelectList(p => p.ProjectId.ToString(), p => p.ProjectName, selectedProject ?? 0, true);
        }

        public static bool AllowEditTags(this Ticket ticket)
        {
            //TODO: is this the best place to put this check?
            var context = DependencyResolver.Current.GetService<TdDomainContext>();
            return context.SecurityProvider.IsTdHelpDeskUser || context.TicketDeskSettings.Permissions.AllowInternalUsersToEditTags;
        }

        public static bool AllowSetOwner(this Ticket ticket)
        {
            //TODO: is this the best place to put this check? Is this one even worth the extension?
            var context = DependencyResolver.Current.GetService<TdDomainContext>();
            return (context.SecurityProvider.IsTdInternalUser && context.TicketDeskSettings.Permissions.AllowInternalUsersToSetOwner) || (context.SecurityProvider.IsTdHelpDeskUser || context.SecurityProvider.IsTdAdministrator);
        }

        public static bool AllowSetAssigned(this Ticket ticket)
        {
            //TODO: is this the best place to put this check? Is this one even worth the extension?
            var context = DependencyResolver.Current.GetService<TdDomainContext>();
            return (context.SecurityProvider.IsTdInternalUser && context.TicketDeskSettings.Permissions.AllowInternalUsersToSetAssigned) || (context.SecurityProvider.IsTdHelpDeskUser || context.SecurityProvider.IsTdAdministrator);

        }

        public static bool AllowEditPriorityList(this Ticket ticket)
        {
            //TODO: is this the best place to put this check?
            var context = DependencyResolver.Current.GetService<TdDomainContext>();
            return (context.SecurityProvider.IsTdAdministrator || context.SecurityProvider.IsTdHelpDeskUser) || (context.SecurityProvider.IsTdInternalUser &&  context.TicketDeskSettings.Permissions.AllowInternalUsersToEditPriority);
        }

        /// <summary>
        /// Commits any pending attachments for the ticket in the file store.
        /// </summary>
        /// <param name="ticket">The ticket.</param>
        /// <param name="tempId">The temporary identifier for pending attachments.</param>
        /// <returns>IEnumerable&lt;System.String&gt;. The list of filenames for all attachments saved</returns>
        public static IEnumerable<string> CommitPendingAttachments(this Ticket ticket, Guid tempId)
        {
            var attachments = TicketDeskFileStore.ListAttachmentInfo(tempId.ToString(), true).ToArray();
            foreach (var attachment in attachments)
            {
                TicketDeskFileStore.MoveFile(
                    attachment.Name,
                    tempId.ToString(),
                    ticket.TicketId.ToString(CultureInfo.InvariantCulture),
                    true,
                    false);
            }
            return attachments.Select(a => a.Name);
        }
    }

}