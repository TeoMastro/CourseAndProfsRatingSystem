import React from 'react';
import { DataGrid, GridColumn} from 'rc-easyui';
import axios from 'axios';
export class Courses extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            dataCourse: this.getCourses(),
            courses: []
        }
    }
    getCourses() {
        axios.get(`https://${window.location.host}/api/course?itemsPerPage=20&page=1`)
            .then(res => {
                const courses = res.data.results;
                this.setState({ courses });
            })
    }
    render() {
        return (
            <div>
                <h2>Courses</h2>
                <DataGrid data={this.state.courses} style={{ height: 550, padding: '15' }}>
                    <GridColumn field="id" title="Id" hidden="true"></GridColumn>
                    <GridColumn field="name" title="Name" align="center"></GridColumn>
                    <GridColumn field="type" title="Type" align="center"></GridColumn>
                </DataGrid>
            </div>
        );
    }
}
export default Courses;