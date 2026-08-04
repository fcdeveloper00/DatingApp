export interface Message{
    id:number;
    senderId:number;
    recipientId:number;
    senderUsername:string;
    recipientUsername:string;
    senderPhotoUrl:string;
    recipientPhotoUrl:string;
    content:string;
    readAt?:Date;
    sentAt:Date;
}