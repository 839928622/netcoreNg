import { Photo } from './Photo';
export interface User {
    id: number;
    username: string;
    knowAs: string;
    age: number;
    gender: string;
    created: Date;
    lastActive: Date;
    photoUrl: string;
    city: string;
    interests?: string;
    introdaction?: string;
    lookingFor?: string;
    photo?: Photo[];
}
