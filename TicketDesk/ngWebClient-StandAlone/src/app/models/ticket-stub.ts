export interface TicketStub {
    ticketId: number;
    title: string;
    status: number;
    ownerId: string;
    assignedTo?: string;
    category: string;
    subcategory: string;
    priority?: string;
    createdDate: string;
    updatedDate?: string;
  }

// Headings for table (displayed to User)
export const columnHeadings: string[] = [
  'TicketID',
  'Title',
  'Status',
  'Priority',
  'Owner',
  'Assigned To',
  'Category',
  'Subcategory',
  'Created Date',
  'Last Updated'
];


export const ticketlistToUserDisplayMap: Map<string, string> = new Map([
  ['unassigned', 'Unassigned Tickets'],
  ['assignedToMe', 'My Assigned Tickets'],
  ['mytickets', 'My Tickets'],
  ['opentickets', 'Open Tickets'],
  ['historytickets', 'Closed Tickets'],
]);
