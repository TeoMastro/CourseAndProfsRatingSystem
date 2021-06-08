import React from 'react';
import { DataGrid, GridColumn} from 'rc-easyui';
import axios from 'axios';
import './Myreviews.css';
export class Myreviews extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            dataReviews: this.getMyReviews(),
            flag: true,
            appsid: '',
            myReviews: []
        }
    }
    getCode() {
        if (this.state.flag) {
            var link = window.location.href;
            this.state.appsid = link.slice(54, 58);
            console.log(link);
            console.log(this.state.appsid);
            this.state.flag = false;
            this.getMyReviews(this.state.appsid);
        }
    }
    getMyReviews(arg) {
        axios.get(`https://${window.location.host}/StudentsReviews?appsId=${arg}&itemsPerPage=20&page=1`)
            .then(res => {
                const myReviews = res.data.results;
                this.setState({ myReviews });
            })
    }
    render() {
        this.getCode();
        return (
            <div className='revdiv1'>
                <div className='revdiv2'>
                    <h5 className='h52'>My reviews</h5>
                    <DataGrid data={this.state.myReviews} filterable columnResizing style={{ height: 550, padding: '15' }}>
                        <GridColumn field="reviewId" title="Id" hidden="true"></GridColumn>
                        <GridColumn field="professorName" title="Professor Name"  align="center"></GridColumn>
                        <GridColumn field="courseName" title="Course Name" align="center"></GridColumn>
                        <GridColumn field="usersSubjectScore" width="100px" title="Course Score" align="center"></GridColumn>
                        <GridColumn field="rating" title="Rating" width="70px" align="center"></GridColumn>
                        <GridColumn field="comments" title="Comments" align="center"></GridColumn>
                    </DataGrid>
                </div>
            </div>
        );
    }
}
export default Myreviews;