import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';
export class NavMenu extends Component {
    static displayName = NavMenu.name;
    constructor(props) {
        super(props);
        this.toggleNavbar = this.toggleNavbar.bind(this);
        this.state = {
            collapsed: true,
        };
    }
    toggleNavbar() {
        this.setState({
            collapsed: !this.state.collapsed
        });
    }

    render() {
        return (
            <header className="head">
                <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
                    <Container>
                        <NavbarBrand style={{ color: 'white', fontWeight: 'bold'}} tag={Link} to="/Main">CourseAndProfsClient</NavbarBrand>
                        <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
                        <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
                            <ul className="navbar-nav flex-grow">
                                {/*<NavItem>*/}
                                {/*    <NavLink tag={Link} className="text-dark" to="/">Home</NavLink>*/}
                                {/*</NavItem>*/}
                                {/*<NavItem>*/}
                                {/*    <NavLink tag={Link} className="text-dark" to="/professors">Professors</NavLink>*/}
                                {/*</NavItem>*/}
                                {/*<NavItem>*/}
                                {/*    <NavLink tag={Link} className="text-dark" to="/courses">Courses</NavLink>*/}
                                {/*</NavItem>*/}
                                <NavItem>
                                    <NavLink><a className="menulink" href="https://login.it.teithe.gr/authorization/?client_id=60ad121a0c09d102ca99dffc&response_type=code&scope=profile&redirect_uri=https://4f85f5d1d03c.ngrok.io">Login</a></NavLink>
                                </NavItem>
                            </ul>
                        </Collapse>
                    </Container>
                </Navbar>
            </header>
        );
    }
}