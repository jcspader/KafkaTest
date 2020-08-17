import React, { Component } from 'react';

export class Message extends Component {
    render() {
        return <h1>{this.props.item.requestId}</h1>;
    }
}
