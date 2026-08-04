import { User } from './user.model';

export class UserParams {
  pageNumber = 1;
  pageSize = 5;
  minAge = 18;
  maxAge = 80;
  gender: string;
  orderBy = 'lastActive';
  
  constructor(user: User | null) {
    this.gender = user?.gender === 'female' ? 'male' : 'female';
  }
}
