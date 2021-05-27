import React from 'react';
import background from "../teithe.png";
import axios from 'axios';
export class Main extends React.Component {
    render() {
        return (
            <div style={{
                backgroundImage: `url(${background})`,
                backgroundPosition: 'center',
                backgroundSize: 'cover',
                backgroundRepeat: 'no-repeat',
                width: '100vw',
                height: '90vh',
                margin: '0',
                padding: '0'
            }}>
                Welcome to course and professors rating system for Iee Ihu.
                The idea for the platform is that authenticated users(students)
                can now anonymously evaluate the professors and their courses 
                and the results will be public to all student society.
                This way, students will now be able to create an impresion of 
                each professor and their respective courses. Students can now know
                what to anticipate before they enroll to each course.
            </div>
        );
    }
}
export default Main;