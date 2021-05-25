import React from 'react';
import { DataGrid, GridColumn, Form, Dialog, Label, LinkButton } from 'rc-easyui';
import axios from 'axios';
export class Home extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            data: this.getRatings(),
            dataProfs: this.getProfessors(),
            profReviews: [],
            //dataCourse: this.getCourses(),
            reviews: [],
            selectValueProf: 0,
            selectValueCourse: 0,
            selectGrade: '',
            selectValueProfId: '',
            selectRating: '',
            selectComments: '',
            flag: true,
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
            title: '',
            closedR: true,
            closed: true,
            
        }
        this.handleChangeProf = this.handleChangeProf.bind(this);
        this.handleChangeCourse = this.handleChangeCourse.bind(this);
        this.handleChangeGrade = this.handleChangeGrade.bind(this);
        this.handleChangeRating = this.handleChangeRating.bind(this);
        this.handleChangeComments = this.handleChangeComments.bind(this);
    }
    getCode() {
        if (this.state.flag == true) {
            var link = window.location.href;
            this.state.code = link.slice(36, 61);
            console.log(this.state.code);
            this.state.flag = false;
        }
    }
    handleChangeProf(e) {
        this.setState({ selectValueProf: e.target.value });
        this.getCourses(e.target.value);
        console.log("boo")
    }
    handleChangeCourse(e) {
        this.setState({ selectValueCourse: e.target.value });
    }
    handleChangeGrade(e) {
        let isnum = /^\d+$/.test(e.target.value);
        if (isnum) {
            this.setState({ selectGrade: e.target.value });
        }
        else {
            window.alert("Give digits only");
        }
    }
    handleChangeRating(e) {
        let isnum = /^\d+$/.test(e.target.value);
        if (isnum) {
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
        axios.get(`https://${window.location.host}/AllProfessorsReviews?itemsPerPage=20&page=1`)
            .then(res => {
                const reviews = res.data.results;
                this.setState({ reviews });
            })
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
                console.log(profReviews);
            })
    }
    getCourses(arg) {
        axios.get(`https://${window.location.host}/api/professor/professorscourses/${arg}`)
            .then(res => {
                const courses = res.data;
                this.setState({ courses });
            })
    }
    submitReview() {
        const course = parseInt(this.state.selectValueCourse);
        const prof = parseInt(this.state.selectValueProf);
        const grade = parseInt(this.state.selectGrade);
        const rating = parseInt(this.state.selectRating);
        const comments = this.state.selectComments;
        if (course == 0 || prof == 0 || grade > 10 || grade < 0 || rating > 10 || rating < 0) {
            window.alert("Check the fields");
        } else {
            axios.post(`https://${window.location.host}/Add?courseId=${course}&professorId=${prof}&usersSubjectScore=${grade}&rating=${rating}&comments=${comments}`)
                .then(res => {
                    window.alert(res.data);
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
    addReview(row) {
        this.setState({
            model: Object.assign({}),
            title: 'Add',
            closed: false
        });
    }
    readReviews(row) {
        this.setState({
            editingRowR: row,
            modelR: Object.assign({}, row),
            title: row.fullName,
            closedR: false
        });
        this.getProfessorsReviews(row.id);
        console.log(row);
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
            <Dialog modal draggable title={title} closed={closed} onClose={() => { this.setState({ closed: true }); this.getRatings() }}>
                <div className="f-full" style={{ padding: '20px 50px' }}>
                    <Form className="f-full"
                        ref={ref => this.form = ref}
                        model={row}
                        rules={rules}
                        onValidate={(errors) => this.setState({ errors: errors })}>
                        <div>
                            <Label htmlFor="cProf" align="top">Select a Professor:</Label>
                            <select value={this.state.selectValueProf} onChange={this.handleChangeProf}>
                                <option value={0}>SELECT</option>
                                {professors.map(professor => <option value={professor.id}>{professor.fullName}</option>)}
                            </select>
                        </div>
                        <div>
                            <Label htmlFor="cCourse" align="top">Select a Course:</Label>
                            <select value={this.state.selectValueCourse} onChange={this.handleChangeCourse}>
                                <option value={0}>SELECT</option>
                                {courses.map(course => <option value={course.id}>{course.name}</option>)}
                            </select>
                        </div>
                        <div style={{ marginTop: 10, marginBottom: 10 }}>
                            <Label htmlFor="tscore" style={{ width: 250 }}>Τι βαθμο πηρατε στο μαθημα;</Label>
                            <input value={this.state.selectGrade} onChange={this.handleChangeGrade} style={{ width: 50 }}></input>
                        </div>
                        <div style={{ marginBottom: 10 }}>
                            <Label htmlFor="treview" style={{ width: 250 }}>Τι βαθμο βαζετε στον καθηγητη;</Label>
                            <input value={this.state.selectRating} onChange={this.handleChangeRating} style={{ width: 50 }}></input>
                            <div className="error">{this.getError('review')}</div>
                        </div>
                        <div style={{ marginBottom: 10 }}>
                            <Label htmlFor="tcomments" align="top">Σχολια:</Label>
                            <textarea value={this.state.selectComments} onChange={this.handleChangeComments} style={{ width: '100%', height: 120 }}></textarea>
                        </div>
                    </Form>
                </div>
                <div className="dialog-button">
                    <LinkButton style={{ width: 80 }} onClick={() => this.submitReview()}>Save</LinkButton>
                    <LinkButton style={{ width: 80 }} onClick={() => { this.setState({ closed: true }); this.getRatings() }}>Close</LinkButton>
                </div>
            </Dialog>
        )
    }
    renderReviews() {
        const row = this.state.modelR;
        const { title, closedR, rules } = this.state;
        return (
            <Dialog modal draggable resizable title={title} closed={closedR} onClose={() => this.setState({ closedR: true })}>
                <div className="f-full" style={{ padding: '20px 50px' }}>
                    <Form className="f-full"
                        ref={ref => this.form = ref}
                        model={row}
                        rules={rules}
                        onValidate={(errors) => this.setState({ errors: errors })}
                    >
                        <div>
                            <DataGrid data={this.state.profReviews} columnResizing style={{ width: 700, height: 400, padding: '15' }}>
                                <GridColumn field="reviewId" title="revId" hidden="true"></GridColumn>
                                <GridColumn field="courseName" title="Course Name" align="center"></GridColumn>
                                <GridColumn field="usersSubjectScore" title="Students's score" align="center" width='60px'></GridColumn>
                                <GridColumn field="rating" title="Rating" align="center" width='60px'></GridColumn>
                                <GridColumn field="comments" title="Comments" align="center"></GridColumn>
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
        return (
            <div>
                <h2>Professor's reviews</h2>
                <DataGrid data={this.state.reviews} style={{ height: 550, padding: '15' }}>
                    <GridColumn field="id" title="PrID" hidden="true"></GridColumn>
                    <GridColumn field="fullName" title="Name" align="center"></GridColumn>
                    <GridColumn field="mail" title="Mail" align="center"></GridColumn>
                    <GridColumn field="department" title="Department" align="center"></GridColumn>
                    <GridColumn field="averageRating" title="Average rating" align="center"></GridColumn>
                    <GridColumn field="act" title="Actions" align="center" width={110}
                        render={({ row }) => (
                            <div>
                                <LinkButton onClick={() => this.readReviews(row)}>Reviews</LinkButton>
                            </div>
                        )}
                    />
                </DataGrid>
                {this.renderDialog()}
                {this.renderReviews()}
                <LinkButton style={{ width: '100%' }} onClick={() => this.addReview()}>Add your review</LinkButton>
            </div>
        );
    }
}
export default Home;