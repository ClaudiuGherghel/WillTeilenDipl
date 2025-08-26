import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Menu } from '../../user/user/menu/menu'

@Component({
  selector: 'app-user',
  imports: [RouterOutlet, Menu],
  standalone: true,
  templateUrl: './user.html',
  styleUrl: './user.css'
})
export class User {

}
