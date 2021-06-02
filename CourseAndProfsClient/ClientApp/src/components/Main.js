import React from 'react';
import { LinkButton } from 'rc-easyui';
import background from "../teithe.png";
import './Main.css';
export class Main extends React.Component {
    render() {
        return (
            <div className='maindiv' style={{backgroundImage: `url(${background})`}}>
                <p className="mainp1">Welcome to course and professors rating system for Iee Ihu.</p>
                <p className="mainp2">The idea for the platform is that authenticated users(students)
                can now anonymously evaluate the professors and their courses 
                and the results will be public to all student society.
                This way, students will now be able to create an impresion of 
                each professor and their respective courses. Students can now know
                what to anticipate before they enroll to each course.<br></br>
                </p>
                <LinkButton className="mainlink" href="https://login.it.teithe.gr/authorization/?client_id=60b80e0992f09141fabd0260&response_type=code&scope=profile&redirect_uri=https://courseandprofs.azurewebsites.net">Login</LinkButton>
            </div>
        );
    }
}
export default Main;