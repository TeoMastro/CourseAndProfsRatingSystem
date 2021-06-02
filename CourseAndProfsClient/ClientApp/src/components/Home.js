import React from 'react';
import { DataGrid, GridColumn, Form, Dialog, Label, NumberBox, LinkButton } from 'rc-easyui';
import axios from 'axios';
import { get } from 'jquery';
import { useHistory } from "react-router-dom";
import './Home.css';
export class Home extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            isAuthorized: false,
            data: [],
            dataProfs: this.getProfessors(), 
            flag: true,
            profReviews: [],
            reviews: [],
            selectValueProf: 0,
            selectValueCourse: 0,
            selectGrade: '',
            selectValueProfId: '',
            selectRating: '',
            selectComments: '',
            code: '',
            professors: [],
            courses: [],
            editingRow: null,
            editingRowR: null,
            model: {},
            modelR: {},
            rules: {
                'itemid': 'required',
                'review': 'required'
            },
            errors: {},
            operators: ["nofilter", "equal", "notequal", "less", "greater"],
            status: [
                { value: null, text: "All" },
                { value: "P", text: "P" },
                { value: "N", text: "N" }
            ],
            title: '',
            closedR: true,
            closed: true,
            access_token: '',
            refresh_token: '',
            userid: '',
            usersname: '',
            userstitle: '',
            usersam: '',
            addYourReviewButtonText: 'Login to add your review'
        }
        this.handleChangeProf = this.handleChangeProf.bind(this);
        this.handleChangeCourse = this.handleChangeCourse.bind(this);
        this.handleChangeGrade = this.handleChangeGrade.bind(this);
        this.handleChangeRating = this.handleChangeRating.bind(this);
        this.handleChangeComments = this.handleChangeComments.bind(this);
    }
    getCode() {
        if (this.state.flag) {
            var link = window.location.href;
            this.state.code = link.slice(47, 72);
            this.state.flag = false;
            this.state.usersname = 'Unauthorized';
            this.getToken(this.state.code);
        }
    }
    handleChangeProf(e) {
        this.setState({ selectValueProf: e.target.value });
        this.getCourses(e.target.value);
    }
    handleChangeCourse(e) {
        this.setState({ selectValueCourse: e.target.value });
    }
    handleChangeGrade(e) {
        let isnum = /^\d+$/.test(e.target.value);
        if (isnum || e.target.value == '') {
            this.setState({ selectGrade: e.target.value });
        }
        else {
            window.alert("Give digits only");
        }
    }
    handleChangeRating(e) {
        let isnum = /^\d+$/.test(e.target.value);
        if (isnum || e.target.value == '') {
            this.setState({ selectRating: e.target.value });
        }
        else {
            window.alert("Give digits only");
        }
    }
    handleChangeComments(e) {
        this.setState({ selectComments: e.target.value });
    }
    getRatings() {
        if (this.state.isAuthorized) {
            axios.get(`https://${window.location.host}/AllProfessorsReviews?itemsPerPage=20&page=1`)
                .then(res => {
                    const reviews = res.data.results;
                    this.setState({ reviews });
                })
        }
    }
    getProfessors() {
        axios.get(`https://${window.location.host}/api/professor?itemsPerPage=20&page=1`)
            .then(res => {
                const professors = res.data.results;
                this.setState({ professors });
            })
    }
    getProfessorsReviews(arg) {
        axios.get(`https://${window.location.host}/ProfessorsReviews?profId=${arg}&itemsPerPage=20&page=1`)
            .then(res => {
                const profReviews = res.data.results;
                this.setState({ profReviews });
            })
    }
    getCourses(arg) {
        axios.get(`https://${window.location.host}/api/professor/professorscourses/${arg}`)
            .then(res => {
                const courses = res.data;
                this.setState({ courses });
            })
    }
    getToken(code) {
        const params = new URLSearchParams();
        params.append('client_id', '60b80e0992f09141fabd0260')
        params.append('client_secret', '5fppkotl3chk7fi7xxk8drp1zjtanhmzdsmaj6axbjtw8pwqqf')
        params.append('grant_type', 'authorization_code')
        params.append('code', code)
        axios.post('https://login.iee.ihu.gr/token', params)
            .then(res => {
                this.state.access_token = res.data.access_token
                this.state.refresh_token = res.data.refresh_token
                this.state.userid = res.data.user
                this.getProfile(res.data.access_token);
            })
    }
    getProfile(ACCESS_TOKEN) {
        axios.get('https://api.iee.ihu.gr/profile', { headers: { 'x-access-token': ACCESS_TOKEN, 'content-type': 'application/json' } })
            .then(res => {
                this.state.usersname = res.data.cn;
                this.state.usersam = res.data.uid;
                this.state.userstitle = res.data.title;
                this.addUpdateUser();
                this.state.addYourReviewButtonText = 'Add your review';
                this.state.isAuthorized = true;
            })
    }
    addUpdateUser() {
        const id = this.state.userid;
        const token = this.state.access_token;
        axios.post(`https://${window.location.host}/api/user?id=${id}&token=${token}`)
            .then(res => {
                console.log("Update users info");
                console.log(res);
            })
    }
    submitReview() {
        const id = this.state.userid;
        const token = this.state.access_token;
        const course = parseInt(this.state.selectValueCourse);
        const prof = parseInt(this.state.selectValueProf);
        const grade = parseInt(this.state.selectGrade);
        const rating = parseInt(this.state.selectRating);
        const comments = this.state.selectComments;
        if (course == 0 || prof == 0 || grade > 10 || grade < 0 || rating > 10 || rating < 0) {
            window.alert("Check the fields");
        } else {
            axios.post(`https://${window.location.host}/Add?courseId=${course}&professorId=${prof}&appsId=${id}&token=${token}&usersSubjectScore=${grade}&rating=${rating}&comments=${comments}`)
                .then(res => {
                    window.alert(res.data);
                })
                .catch(res => {
                    window.alert(res.response.data);
                })
            this.getRatings();
        }
    }
    getError(name) {
        const { errors } = this.state;
        if (!errors) {
            return null;
        }
        return errors[name] && errors[name].length
            ? errors[name][0]
            : null;
    }
    addReview() {
        if (this.state.isAuthorized) {
            this.setState({
                model: Object.assign({}),
                title: 'Add your review',
                closed: false
            });
        }
        else {
            window.open("https://login.it.teithe.gr/authorization/?client_id=60b80e0992f09141fabd0260&response_type=code&scope=profile&redirect_uri=https://courseandprofs.azurewebsites.net", "_self");
        }
    }
    readReviews(row) {
        this.setState({
            editingRowR: row,
            modelR: Object.assign({}, row),
            title: row.fullName,
            closedR: false
        });
        this.getProfessorsReviews(row.id);
    }
    myReviews() {
        var id = this.state.userid;
        var link = "https://courseandprofs.azurewebsites.net/myreviews?id=" + id;
        window.location.href = link;
    }
    saveRow() {
        this.form.validate(() => {
            if (this.form.valid()) {
                let row = Object.assign({}, this.state.editingRow, this.state.model);
                let data = this.state.data.slice();
                let index = data.indexOf(this.state.editingRow);
                data.splice(index, 1, row);
                this.setState({
                    data: data,
                    closed: true
                })
            }
        })
    }
    clearValue() {
        this.setState({ value: null })
    }
    deleteRow(row) {
        this.setState({
            data: this.state.data.filter(r => r !== row)
        })
    }
    renderDialog() {
        const row = this.state.model;
        const { title, closed, rules } = this.state;
        let professors = this.state.professors;
        let courses = this.state.courses;
        return (
            <Dialog className="dial1" borderType="none" modal closed={closed} onClose={() => { this.setState({ closed: true }); this.getRatings() }}>
                <div className="popup1">
                    <Form className="f-full"
                        ref={ref => this.form = ref}
                        model={row}
                        rules={rules}
                        onValidate={(errors) => this.setState({ errors: errors })}>
                        <div className="div2">
                            <h3 className="h31">{title}</h3>
                        </div>
                        <div className="div2"> 
                            <Label className="lab1" htmlFor="cProf" align="top">Select a Professor:</Label>
                            <select className="texare1" value={this.state.selectValueProf} onChange={this.handleChangeProf}>
                                <option value={0}>SELECT</option>
                                {professors.map(professor => <option value={professor.id}>{professor.fullName}</option>)}
                            </select>
                        </div>
                        <div className="div2"> 
                            <Label className="lab1" htmlFor="cCourse" align="top">Select a Course:</Label>
                            <select className="texare1" value={this.state.selectValueCourse} onChange={this.handleChangeCourse}>
                                <option value={0}>SELECT</option>
                                {courses.map(course => <option value={course.id}>{course.name}</option>)}
                            </select>
                        </div>
                        <div className="div2">
                            <Label className="lab2" htmlFor="tscore">Grade you got in the lesson:</Label>
                            <input className="texare1" value={this.state.selectGrade} onChange={this.handleChangeGrade} style={{ width: 50 }}></input>
                        </div>
                        <div className="div2">
                            <Label className="lab2" htmlFor="treview">Degree you give to the teacher:</Label>
                            <input className="texare1" value={this.state.selectRating} onChange={this.handleChangeRating} style={{ width: 50 }}></input>
                        </div>
                        <div className="error">{this.getError('review')}</div>
                        <div className="div2">
                            <Label className="lab3" htmlFor="tcomments" align="top">Comments:</Label>
                            <textarea className="texare2" value={this.state.selectComments} onChange={this.handleChangeComments}></textarea>
                        </div>
                    </Form>
                </div>
                <div className="dialog-button">
                    <LinkButton className="linbut2" style={{ width: 80 }} onClick={() => this.submitReview()}>Save</LinkButton>
                    <LinkButton className="linbut2" style={{ width: 80 }} onClick={() => { this.setState({ closed: true }); this.getRatings() }}>Close</LinkButton>
                </div>
            </Dialog>
        )
    }
    renderReviews() {
        const row = this.state.modelR;
        const { title, closedR, rules } = this.state;
        return (
            <Dialog className="dial1" borderType="none" modal closed={closedR} onClose={() => this.setState({ closedR: true })}>
                <div className="popup1">
                    <Form className="f-full"
                        ref={ref => this.form = ref}
                        model={row}
                        rules={rules}
                        onValidate={(errors) => this.setState({ errors: errors })}>
                        <div className="div3">
                            <h3 className="h31">{title}</h3>
                            <LinkButton className="linbut2" style={{ width: 80 }} onClick={() => { this.setState({ closedR: true })}}>Close</LinkButton>
                        </div>
                        <div className="div2"> 
                            <DataGrid className="dagr1" data={this.state.profReviews} filterable columnMoving multiSort columnResizing>
                                <GridColumn field="reviewId" title="revId" hidden="true"></GridColumn>
                                <GridColumn field="courseName" title="Course Name" align="center" sortable></GridColumn>
                                <GridColumn field="usersSubjectScore" title="Students's score" align="center" width='60px' sortable></GridColumn>
                                <GridColumn field="rating" title="Rating" align="center" width='60px' sortable
                                    filterOperators={this.state.operators}
                                    filter={() => <NumberBox></NumberBox>}></GridColumn>
                                <GridColumn field="comments" title="Comments" align="center" sortable></GridColumn>
                            </DataGrid>
                        </div>
                    </Form>
                </div>
            </Dialog>
        )
    }
    render() {
        const clearValue = () => {
            this.setState({ value: null })
        }
        this.getCode();
        this.state.data = this.getRatings();
        return (
            <div className='homediv'>
                <div className='homediv2'>
                    <div className='div1'>
                        <div >
                            <h5 className='h51'>Logged in as {this.state.usersname}<br></br></h5>
                        </div>
                        <div>
                            <LinkButton style={{ marginRight: 40 }} className='linbut1' onClick={() => this.myReviews()}>My reviews</LinkButton>
                            <LinkButton className='linbut1' onClick={() => this.addReview()}>{this.state.addYourReviewButtonText}</LinkButton>
                        </div>
                    </div>
                    <DataGrid className="dagr1" data={this.state.reviews} filterable columnMoving multiSort columnResizing>
                        <GridColumn field="id" title="PrID" hidden="true"></GridColumn>
                        <GridColumn field="fullName" title="Name" align="center" sortable></GridColumn>
                        <GridColumn field="mail" title="Mail" align="center" sortable></GridColumn>
                        <GridColumn field="department" title="Department" align="center" sortable></GridColumn>
                        <GridColumn field="averageRating" title="Average rating" align="center" sortable
                            filterOperators={this.state.operators}
                            filter={() => <NumberBox></NumberBox>}></GridColumn>
                        <GridColumn field="act" title="Reviews" align="center" width={110}
                            filter={() => <label></label>}
                            render={({ row }) => (
                                <div>
                                    <LinkButton className="linbut3" onClick={() => this.readReviews(row)}>Reviews</LinkButton>
                                </div>
                            )}
                        />
                    </DataGrid>
                    {this.renderDialog()}
                    {this.renderReviews()}
                </div>
            </div>
        );
    }
}
export default Home;