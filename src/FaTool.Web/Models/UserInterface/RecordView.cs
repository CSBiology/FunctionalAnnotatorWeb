using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Linq;
using System.Globalization;

namespace FaTool.Web.Models.UserInterface
{

    public interface IRecordView
    {
        string Caption { get; }
        int NumberOfFields { get; }
        string GetFieldName(int fieldIdx);
        bool HasSubValues(int fieldIdx);
        ActionList Actions { get; }
        bool IsTableView { get; }
    }

    public interface IObjectView : IRecordView
    {
        string GetValue(int fieldIdx);
        IEnumerable<string> GetSubValues(int fieldIdx);
    }

    public interface IObjectView<TValue> : IObjectView
    {
        TValue Value { get; }
    }

    public interface ITableView : IRecordView
    {
        int NumberOfRows { get; }
        IObjectView GetRow(int rowIdx);
        bool HasRowActions { get; }
    }

    public interface ITableView<TValue> : ITableView
    {
        new IObjectView<TValue> GetRow(int rowIdx);

        void AddRowAction(Func<TValue, ActionLink> createLink);
    }

    public class ObjectView<TValue> : IObjectView<TValue>
    {
        private readonly IFormatProvider format;
        private readonly RecordFieldCollection<TValue> fields;
        private readonly TValue value;
        private readonly ActionList actions;

        public ObjectView(TValue value, RecordFieldCollection<TValue> fields, IFormatProvider format)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (fields == null)
                throw new ArgumentNullException("fields");
            if (format == null)
                throw new ArgumentNullException("format");

            this.format = format;
            this.value = value;
            this.fields = fields;
            this.actions = new ActionList("Actions");
            this.Caption = string.Format("{0} Properties", typeof(TValue).Name);
        }

        public ObjectView(TValue value, IFormatProvider format)
            : this(value, new RecordFieldCollection<TValue>(), format) { }

        public ObjectView(TValue value) : this(value, new CultureInfo("en-US")) { }

        public RecordFieldCollection<TValue> Fields { get { return fields; } }

        public void AddField(
            string name,
            params Expression<Func<TValue, object>>[] properties)
        {
            fields.AddField(name, properties);
        }

        public TValue Value { get { return value; } }

        #region IRecordView Members

        public string Caption { get; set; }

        public int NumberOfFields
        {
            get { return fields.Count; }
        }

        public string GetFieldName(int fieldIdx)
        {
            return fields[fieldIdx].Name;
        }

        public bool HasSubValues(int fieldIdx)
        {
            return fields[fieldIdx].HasSubValues;
        }

        public bool IsTableView { get { return false; } }

        #endregion

        #region IObjectView Members

        public string GetValue(int fieldIdx)
        {
            return fields[fieldIdx].GetValue(value, format);
        }

        public IEnumerable<string> GetSubValues(int fieldIdx)
        {
            return fields[fieldIdx].GetSubValues(value, format);
        }

        public ActionList Actions
        {
            get { return actions; }
        }

        #endregion

    }

    public class TableView<TValue> : ITableView<TValue>
    {
        private readonly IFormatProvider format;
        private readonly RecordFieldCollection<TValue> fields;
        private readonly IList<ObjectView<TValue>> rows;
        private readonly ActionList actions;

        public TableView(IEnumerable<TValue> rowValues, IFormatProvider format)
        {
            if (format == null)
                throw new ArgumentNullException("format");
            if (rowValues == null)
                throw new ArgumentNullException("rowValues");

            this.format = format;
            this.fields = new RecordFieldCollection<TValue>();
            this.rows = rowValues.Select(x => new ObjectView<TValue>(x, fields, format)).ToList();
            this.actions = new ActionList("Actions");
            this.Caption = string.Format("{0} Table", typeof(TValue).Name);
        }

        public TableView(IEnumerable<TValue> rowValues)
            : this(rowValues, new CultureInfo("en-US")) { }

        public void AddField(
            string name,
            params Expression<Func<TValue, object>>[] properties)
        {
            fields.AddField(name, properties);
        }

        #region IRecordView Members

        public string Caption { get; set; }

        public int NumberOfFields
        {
            get { return fields.Count; }
        }

        public string GetFieldName(int fieldIdx)
        {
            return fields[fieldIdx].Name;
        }

        public bool HasSubValues(int fieldIdx)
        {
            return fields[fieldIdx].HasSubValues;
        }

        #endregion

        #region ITableView<TValue> Members

        public IObjectView<TValue> GetRow(int rowIdx)
        {
            return rows[rowIdx];
        }

        public void AddRowAction(
            Func<TValue, ActionLink> createLink)
        {
            for (int rowIdx = 0; rowIdx < NumberOfRows; rowIdx++)
            {
                var row = GetRow(rowIdx);
                var link = createLink.Invoke(row.Value);
                if (link != null)
                    row.Actions.Add(link);
            }
        }

        #endregion

        #region ITableView Members

        public int NumberOfRows
        {
            get { return rows.Count; }
        }

        public bool HasRowActions
        {
            get { return rows.Any(x => x.Actions.Count > 0); }
        }

        IObjectView ITableView.GetRow(int rowIdx)
        {
            return rows[rowIdx];
        }

        public ActionList Actions { get { return actions; } }

        public bool IsTableView { get { return true; } }

        #endregion
    }

    public sealed class RecordField<TValue>
    {
        private readonly IList<Func<TValue, object>> properties;
        private readonly string name;

        public RecordField(
            string name,
            params Expression<Func<TValue, object>>[] properties)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            if (properties == null)
                throw new ArgumentNullException("properties");

            this.name = name;
            this.properties = properties.Select(x => x.Compile()).ToList();
        }

        public string GetValue(TValue value, IFormatProvider format)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return properties
                .Select(x => Convert.ToString(x.Invoke(value), format))
                .FirstOrDefault();
        }

        public IEnumerable<string> GetSubValues(TValue value, IFormatProvider format)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return properties
                .Skip(1)
                .Select(x => Convert.ToString(x.Invoke(value), format));
        }

        public string Name
        {
            get { return name; }
        }

        public bool HasSubValues
        {
            get { return properties.Count > 1; }
        }
    }

    public sealed class RecordFieldCollection<TValue>
        : KeyedCollection<string, RecordField<TValue>>
    {

        public RecordFieldCollection() { }

        protected override string GetKeyForItem(RecordField<TValue> item)
        {
            return item.Name;
        }

        public void AddField(
            string name,
            params Expression<Func<TValue, object>>[] properties)
        {
            var field = new RecordField<TValue>(name, properties);
            Add(field);
        }
    }

}